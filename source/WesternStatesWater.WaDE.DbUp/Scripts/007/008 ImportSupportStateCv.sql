-- Create new SiteTableType with StateCV
CREATE TYPE [Core].[SiteTableType_new] AS TABLE(
    [SiteUUID] [nvarchar](200) NULL,
    [SiteNativeID] [nvarchar](50) NULL,
    [SiteName] [nvarchar](500) NULL,
    [USGSSiteID] [nvarchar](250) NULL,
    [SiteTypeCV] [nvarchar](100) NULL,
    [Longitude] [nvarchar](100) NULL,
    [Latitude] [nvarchar](100) NULL,
    [Geometry] [nvarchar](max) NULL,
    [CoordinateMethodCV] [nvarchar](100) NULL,
    [CoordinateAccuracy] [nvarchar](255) NULL,
    [GNISCodeCV] [nvarchar](50) NULL,
    [EPSGCodeCV] [nvarchar](50) NULL,
    [HUC8] [nvarchar](20) NULL,
    [HUC12] [nvarchar](20) NULL,
    [County] [nvarchar](20) NULL,
    [PODorPOUSite] [nvarchar](50) NULL,
    [RegulatoryOverlayUUDIs] [nvarchar](max) NULL,
    [WaterSourceUUIDs] [nvarchar](max) NULL,
    [StateCV] [nvarchar](2) NULL
)
GO

-- Update the UUDT
EXEC Core.UpdateUUDT 'Core', 'SiteTableType';
GO

-- Recreate LoadSites procedure with StateCV
CREATE OR ALTER PROCEDURE [Core].[LoadSites]
(
    @RunId NVARCHAR(250),
    @SiteTable Core.SiteTableType READONLY
)
AS
BEGIN
SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
INTO #TempSiteData
FROM @SiteTable;

--take every RegulatoryOverlayUUID and joint it to a RegulatoryOverlayID
SELECT
    sd.RowNumber,
    RegulatoryOverlayID = rodim.RegulatoryOverlayID
INTO
    #TempRegulatorOverlayBridgeData
FROM
    #TempSiteData sd
    CROSS APPLY STRING_SPLIT(sd.RegulatoryOverlayUUDIs, ',') ro
            LEFT OUTER JOIN Core.RegulatoryOverlay_dim rodim ON TRIM(ro.[value]) = rodim.RegulatoryOverlayUUID
WHERE
    ro.[Value] IS NOT NULL
  AND LEN(TRIM(sd.RegulatoryOverlayUUDIs)) > 0
  AND LEN(TRIM(ro.[Value])) > 0;

SELECT
    sd.RowNumber,
    WaterSourceID = wsdim.WaterSourceID
INTO
    #TempWaterSourceBridgeData
FROM
    #TempSiteData sd
    CROSS APPLY STRING_SPLIT(sd.WaterSourceUUIDs, ',') ws
            LEFT OUTER JOIN Core.WaterSources_dim wsdim ON TRIM(ws.[value]) = wsdim.WaterSourceUUID
WHERE
    ws.[Value] IS NOT NULL
  AND LEN(TRIM(sd.WaterSourceUUIDs)) > 0
  AND LEN(TRIM(ws.[Value])) > 0;

--data validation
WITH q1 AS
         (
             SELECT 'SiteUUID Not Valid' Reason, *
             FROM #TempSiteData
             WHERE SiteUUID IS NULL
             UNION ALL
             SELECT 'SiteName Not Valid' Reason, *
             FROM #TempSiteData
             WHERE SiteName IS NULL
             UNION ALL
             SELECT 'CoordinateMethodCV Not Valid' Reason, *
             FROM #TempSiteData
             WHERE CoordinateMethodCV IS NULL
             UNION ALL
             SELECT 'EPSGCodeCV Not Valid' Reason, *
             FROM #TempSiteData
             WHERE EPSGCodeCV IS NULL
             UNION ALL
             SELECT 'PODorPOUSite found POD record with missing longitude and latitude' Reason, *
             FROM #TempSiteData
             WHERE PODorPOUSite = 'POD' AND
                 (Longitude IS NULL AND Latitude IS NULL)
         )
SELECT * INTO #TempErrorSiteRecords FROM q1;

--if we have errors, insert them and bail out
IF EXISTS(SELECT 1 FROM #TempErrorSiteRecords)
BEGIN
INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
VALUES ('Sites', @RunId, (SELECT * FROM #TempErrorSiteRecords FOR JSON PATH));
RETURN 1;
END

CREATE TABLE #TempSiteRecords(SiteID BIGINT, RowNumber BIGINT);

--merge the data
MERGE INTO Core.Sites_dim AS Target USING #TempSiteData AS Source
    ON Target.SiteUUID = Source.SiteUUID
    WHEN MATCHED THEN
        UPDATE SET
            SiteNativeID = Source.SiteNativeID
            ,SiteName = Source.SiteName
            ,USGSSiteID = Source.USGSSiteID
            ,SiteTypeCV = Source.SiteTypeCV
            ,Longitude = CONVERT(FLOAT, Source.Longitude)
            ,Latitude = CONVERT(FLOAT, Source.Latitude)
            ,SitePoint = geometry::STGeomFromText('POINT(' + Source.Longitude + ' ' + Source.Latitude + ')', 4326)
                ,[Geometry] = geometry::STGeomFromText(Source.[Geometry], 4326)
                ,CoordinateMethodCV = Source.CoordinateMethodCV
                ,CoordinateAccuracy = Source.CoordinateAccuracy
                ,GNISCodeCV = Source.GNISCodeCV
                ,EPSGCodeCV = Source.EPSGCodeCV
                ,HUC8 = Source.HUC8
                ,HUC12 = Source.HUC12
                ,County = Source.County
                ,PODorPOUSite = Source.PODorPOUSite
                ,StateCV = Source.StateCV
                
        WHEN NOT MATCHED THEN
            INSERT
                (SiteUUID
                ,SiteNativeID
                ,SiteName
                ,USGSSiteID
                ,SiteTypeCV
                ,Longitude
                ,Latitude
                ,SitePoint
                ,[Geometry]
                ,CoordinateMethodCV
                ,CoordinateAccuracy
                ,GNISCodeCV
                ,EPSGCodeCV
                ,HUC8
                ,HUC12
                ,County
                ,PODorPOUSite
                ,StateCV
                )
            VALUES
                (Source.SiteUUID
                ,Source.SiteNativeID
                ,Source.SiteName
                ,Source.USGSSiteID
                ,Source.SiteTypeCV
                ,CONVERT(FLOAT, Source.Longitude)
                ,CONVERT(FLOAT, Source.Latitude)
                ,geometry::STGeomFromText('POINT(' + Source.Longitude + ' ' + Source.Latitude + ')', 4326)
                ,geometry::STGeomFromText(Source.[Geometry], 4326)
                ,Source.CoordinateMethodCV
                ,Source.CoordinateAccuracy
                ,Source.GNISCodeCV
                ,Source.EPSGCodeCV
                ,Source.HUC8
                ,Source.HUC12
                ,Source.County
                ,Source.PODorPOUSite
                ,Source.StateCV
                )
            OUTPUT
                inserted.SiteID
                ,Source.RowNumber
            INTO
                #TempSiteRecords;

-- insert into regulatory overlay bridge
SELECT DISTINCT
    ro.RegulatoryOverlayID,
    tsr.SiteID
INTO #TempRegulatorOverlayBridge
FROM
    #TempSiteRecords tsr INNER JOIN
    #TempRegulatorOverlayBridgeData ro ON ro.RowNumber = tsr.RowNumber;

WITH q2 as (SELECT * FROM Core.RegulatoryOverlayBridge_Sites_fact robsf WHERE robsf.SiteID IN (SELECT SiteID FROM #TempSiteRecords))
    MERGE INTO q2 AS Target USING #TempRegulatorOverlayBridge AS Source
ON Target.RegulatoryOverlayID = Source.RegulatoryOverlayID AND
    Target.SiteID = Source.SiteID

    WHEN NOT MATCHED THEN
INSERT (SiteID, RegulatoryOverlayID)
VALUES (Source.SiteID, Source.RegulatoryOverlayID)

    WHEN NOT MATCHED BY SOURCE THEN
DELETE;

-- insert into water source bridge
SELECT DISTINCT
    ws.WaterSourceID,
    tsr.SiteID
INTO #TempWaterSourceBridge
FROM
    #TempSiteRecords tsr INNER JOIN
    #TempWaterSourceBridgeData ws ON ws.RowNumber = tsr.RowNumber;

WITH q2 as (SELECT * FROM Core.WaterSourceBridge_Sites_fact wssf WHERE wssf.SiteID IN (SELECT SiteID FROM #TempSiteRecords))
    MERGE INTO q2 AS Target USING #TempWaterSourceBridge AS Source
ON Target.WaterSourceID = Source.WaterSourceID AND
    Target.SiteID = Source.SiteID

    WHEN NOT MATCHED THEN
INSERT (SiteID, WaterSourceID)
VALUES (Source.SiteID, Source.WaterSourceID)

    WHEN NOT MATCHED BY SOURCE THEN
DELETE;

RETURN 0;
END
GO