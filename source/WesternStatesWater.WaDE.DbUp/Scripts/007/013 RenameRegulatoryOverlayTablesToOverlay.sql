/*
    This script renames RegulatoryOverlay to Overlay, and it's related tables, columns, user-defined table types, and views.
    
    1. Rename table columns
    2. Recreate User-Defined Table Types
    3. Update Stored Procedures
    4. Update Views
    5. Rename tables
*/

/*
    **********************************************************************************
    Step 1: Rename table columns
    **********************************************************************************
*/

DROP VIEW [dbo].[OverlaysView]

EXEC sp_rename 'Core.RegulatoryOverlay_dim.RegulatoryOverlayID', 'OverlayID', 'COLUMN';
EXEC sp_rename 'Core.RegulatoryOverlay_dim.RegulatoryOverlayUUID', 'OverlayUUID', 'COLUMN';
EXEC sp_rename 'Core.RegulatoryOverlay_dim.RegulatoryOverlayNativeID', 'OverlayNativeID', 'COLUMN';
EXEC sp_rename 'Core.RegulatoryOverlay_dim.RegulatoryName', 'OverlayName', 'COLUMN';
EXEC sp_rename 'Core.RegulatoryOverlay_dim.RegulatoryDescription', 'OverlayDescription', 'COLUMN';
EXEC sp_rename 'Core.RegulatoryOverlay_dim.RegulatoryOverlayTypeCV', 'OverlayTypeCV', 'COLUMN';

EXEC sp_rename 'Core.RegulatoryOverlayBridge_Sites_fact.RegulatoryOverlayBridgeID', 'OverlayBridgeID', 'COLUMN';
EXEC sp_rename 'Core.RegulatoryOverlayBridge_Sites_fact.RegulatoryOverlayID', 'OverlayID', 'COLUMN';

EXEC sp_rename 'Core.RegulatoryReportingUnits_fact.RegulatoryOverlayID', 'OverlayID', 'COLUMN';

/*
    **********************************************************************************
    Step 2: Recreate User-Defined Table Types
    **********************************************************************************
*/

/* Updating type name and fixing typo */
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
    [OverlayUUIDs] [nvarchar](max) NULL,
    [WaterSourceUUIDs] [nvarchar](max) NULL,
    [StateCV] [nvarchar](2) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'SiteTableType';
GO

CREATE TYPE [Core].[OverlayTableType] AS TABLE(
    [OverlayUUID] [nvarchar](250) NULL,
    [OverlayNativeID] [nvarchar](250) NULL,
    [OverlayName] [nvarchar](50) NULL,
    [OverlayDescription] [nvarchar](max) NULL,
    [RegulatoryStatusCV] [nvarchar](50) NULL,
    [OversightAgency] [nvarchar](250) NULL,
    [RegulatoryStatute] [nvarchar](500) NULL,
    [RegulatoryStatuteLink] [nvarchar](500) NULL,
    [StatutoryEffectiveDATE] [date] NULL,
    [StatutoryEndDATE] [date] NULL,
    [OverlayTypeCV] [nvarchar](100) NULL,
    [WaterSourceTypeCV] [nvarchar](100) NULL
)
GO

CREATE TYPE [Core].[OverlayReportingUnitsTableType] AS TABLE(
    [OrganizationUUID] [nvarchar](250) NULL,
    [OverlayUUID] [nvarchar](250) NULL,
    [ReportingUnitUUID] [nvarchar](250) NULL,
    [DataPublicationDate] [nvarchar](250) NULL
)
GO


/*
    **********************************************************************************
    Step 2: Update Stored Procedures
    **********************************************************************************
*/

DROP PROCEDURE [Core].[LoadRegulatoryOverlays]
GO
CREATE PROCEDURE [Core].[LoadOverlays]
(
    @RunId NVARCHAR(250),
    @OverlayTable Core.OverlayTableType READONLY
)
AS
BEGIN
SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
INTO #TempOverlayData
FROM @OverlayTable;

--data validation
WITH q1 AS
         (
             SELECT 'OverlayUUID Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OverlayUUID IS NULL
             UNION ALL
             SELECT 'OverlayName Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OverlayName IS NULL
             UNION ALL
             SELECT 'OverlayDescription Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OverlayDescription IS NULL
             UNION ALL
             SELECT 'OverlayStatusCV Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE RegulatoryStatusCV IS NULL
             UNION ALL
             SELECT 'OversightAgency Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OversightAgency IS NULL
             UNION ALL
             SELECT 'StatutoryEffectiveDate Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE StatutoryEffectiveDate IS NULL
         )
SELECT * INTO #TempErrorOverlayRecords FROM q1;

--if we have errors, insert them and bail out
IF EXISTS(SELECT 1 FROM #TempErrorOverlayRecords)
BEGIN
INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
VALUES ('Overlays', @RunId, (SELECT * FROM #TempErrorOverlayRecords FOR JSON PATH));
RETURN 1;
END

--merge into Core.Overlay_dim
MERGE INTO Core.Overlay_dim AS Target USING #TempOverlayData AS Source ON
    Target.OverlayUUID = Source.OverlayUUID
    WHEN MATCHED THEN
        UPDATE SET
            OverlayNativeID = Source.OverlayNativeID
            ,OverlayName = Source.OverlayName
            ,OverlayDescription = Source.OverlayDescription
            ,RegulatoryStatusCV = Source.RegulatoryStatusCV
            ,OversightAgency = Source.OversightAgency
            ,RegulatoryStatute = Source.RegulatoryStatute
            ,RegulatoryStatuteLink = Source.RegulatoryStatuteLink
            ,StatutoryEffectiveDate = Source.StatutoryEffectiveDate
            ,StatutoryEndDate = Source.StatutoryEndDate
    WHEN NOT MATCHED THEN
        INSERT
            (OverlayUUID
                ,OverlayNativeID
                ,OverlayName
                ,OverlayDescription
                ,RegulatoryStatusCV
                ,OversightAgency
                ,RegulatoryStatute
                ,RegulatoryStatuteLink
                ,StatutoryEffectiveDate
                ,StatutoryEndDate
                ,OverlayTypeCV
                ,WaterSourceTypeCV)
            VALUES
                (Source.OverlayUUID
                ,Source.OverlayNativeID
                ,Source.OverlayName
                ,Source.OverlayDescription
                ,Source.RegulatoryStatusCV
                ,Source.OversightAgency
                ,Source.RegulatoryStatute
                ,Source.RegulatoryStatuteLink
                ,Source.StatutoryEffectiveDate
                ,Source.StatutoryEndDate
                ,Source.OverlayTypeCV
                ,Source.WaterSourceTypeCV);
RETURN 0;
END
GO


/************************************************************************************/

DROP PROCEDURE [Core].[LoadRegulatoryReportingUnits]
GO
CREATE PROCEDURE [Core].[LoadOverlayReportingUnits]
(
    @RunId NVARCHAR(250),
    @OverlayReportingUnitsTableType Core.OverlayReportingUnitsTableType READONLY
)
AS
BEGIN
SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
INTO #TempOverlayReportingUnitData
FROM @OverlayReportingUnitsTableType;

SELECT
    rrud.*
     ,dd.DateID
     ,org.OrganizationID OrgID
     ,ro.OverlayID RuID
     ,ru.ReportingUnitID ReID


INTO
    #TempJoinedOverlayReportingUnitData
FROM
    #TempOverlayReportingUnitData rrud
    LEFT OUTER JOIN Core.Date_dim dd ON rrud.DataPublicationDate = dd.[Date]
    LEFT OUTER JOIN Core.Organizations_dim org ON rrud.OrganizationUUID = org.OrganizationUUID
    LEFT OUTER JOIN Core.Overlay_dim ro ON rrud.OverlayUUID = ro.OverlayUUID
    LEFT OUTER JOIN Core.ReportingUnits_dim ru ON rrud.ReportingUnitUUID = ru.ReportingUnitUUID;

--data validation
WITH q1 AS
         (
             SELECT 'OrganizationUUID Not Valid' Reason, *
             FROM  #TempJoinedOverlayReportingUnitData
             WHERE OrganizationUUID IS NULL
             UNION ALL
             SELECT 'OverlayUUID Not Valid' Reason, *
             FROM  #TempJoinedOverlayReportingUnitData
             WHERE OverlayUUID IS NULL
             UNION ALL
             SELECT 'ReportingUnitUUID Not Valid' Reason, *
             FROM  #TempJoinedOverlayReportingUnitData
             WHERE ReportingUnitUUID IS NULL
             UNION ALL
             SELECT 'DataPublicationDate Not Valid' Reason, *
             FROM  #TempJoinedOverlayReportingUnitData
             WHERE DataPublicationDate IS NULL

         )
SELECT * INTO #TempErrorOverlayReportingUnitRecords FROM q1;

--if we have errors, insert them and bail out
IF EXISTS(SELECT 1 FROM #TempErrorOverlayReportingUnitRecords)
BEGIN
INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
VALUES ('OverlayReportingUnits', @RunId, (SELECT * FROM #TempErrorOverlayReportingUnitRecords FOR JSON PATH));
RETURN 1;
END;

WITH q1 AS
         (
             SELECT
                 jrrud.DateID
                  ,jrrud.OrgID
                  ,jrrud.RuID
                  ,jrrud.ReID


             FROM
                 #TempJoinedOverlayReportingUnitData jrrud)

     --merge the data
    MERGE INTO Core.OverlayReportingUnits_fact AS Target USING q1 AS Source ON
    Target.OrganizationID = Source.OrgID AND
    Target.OverlayID = Source.RuID AND
    Target.ReportingUnitID = Source.ReID
    WHEN MATCHED THEN
UPDATE SET

    DataPublicationDateID = Source.DateID

    WHEN NOT MATCHED THEN
INSERT
(OrganizationID
,OverlayID
,ReportingUnitID
,DataPublicationDateID)
VALUES
    (Source.OrgID
        ,Source.RuID
        ,Source.ReID
        ,Source.DateID);
RETURN 0;
END
GO

/************************************************************************************/


ALTER PROCEDURE [Core].[LoadSites]
(
    @RunId NVARCHAR(250),
    @SiteTable Core.SiteTableType READONLY
)
AS
BEGIN
SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
INTO #TempSiteData
FROM @SiteTable;

--take every OverlayUUID and joint it to an OverlayID
SELECT
    sd.RowNumber,
    OverlayID = rodim.OverlayID
INTO
    #TempOverlayBridgeData
FROM
    #TempSiteData sd
    CROSS APPLY STRING_SPLIT(sd.OverlayUUIDs, ',') ro
            LEFT OUTER JOIN Core.Overlay_dim rodim ON TRIM(ro.[value]) = rodim.OverlayUUID
WHERE
    ro.[Value] IS NOT NULL
  AND LEN(TRIM(sd.OverlayUUIDs)) > 0
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
                )
            OUTPUT
                inserted.SiteID
                ,Source.RowNumber
            INTO
                #TempSiteRecords;

-- insert into overlay bridge
SELECT DISTINCT
    ro.OverlayID,
    tsr.SiteID
INTO #TempOverlayBridge
FROM
    #TempSiteRecords tsr INNER JOIN
    #TempOverlayBridgeData ro ON ro.RowNumber = tsr.RowNumber;

WITH q2 as (SELECT * FROM Core.OverlayBridge_Sites_fact robsf WHERE robsf.SiteID IN (SELECT SiteID FROM #TempSiteRecords))
    MERGE INTO q2 AS Target USING #TempOverlayBridge AS Source
ON Target.OverlayID = Source.OverlayID AND
    Target.SiteID = Source.SiteID

    WHEN NOT MATCHED THEN
INSERT (SiteID, OverlayID)
VALUES (Source.SiteID, Source.OverlayID)

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



/************************************************************************************/





/************************************************************************************/

EXEC sp_rename 'Core.RegulatoryOverlay_dim', 'Overlay_dim';
EXEC sp_rename 'Core.RegulatoryOverlayBridge_Sites_fact', 'OverlayBridge_Sites_fact';
EXEC sp_rename 'Core.RegulatoryReportingUnits_fact', 'OverlayReportingUnits_fact';
EXEC sp_rename 'CVs.RegulatoryOverlayType', 'OverlayType';

GO
CREATE VIEW [dbo].[OverlaysView] WITH SCHEMABINDING AS
SELECT
    o.ReportingUnitUUID,
    rud.Geometry AS geometry,
    o.OverlayTypeWaDEName,
    rud.StateCV AS State,
    o.WaterSourceTypeWaDEName
FROM (
         SELECT
             rud.ReportingUnitID,
             MIN(rud.ReportingUnitUUID) AS ReportingUnitUUID,
             STRING_AGG(CAST(rotcv.WaDEName AS nvarchar(max)), '||') AS OverlayTypeWaDEName,
             STRING_AGG(CAST(wstcv.WaDEName AS nvarchar(max)), '||') AS WaterSourceTypeWaDEName
         FROM Core.ReportingUnits_dim rud
                  INNER JOIN Core.OverlayReportingUnits_fact rruf ON rud.ReportingUnitID = rruf.ReportingUnitID
                  INNER JOIN Core.Overlay_dim rod ON rod.OverlayID = rruf.OverlayID
                  LEFT OUTER JOIN CVs.OverlayType rotcv ON rotcv.Name = rod.OverlayTypeCV
                  LEFT OUTER JOIN CVs.WaterSourceType wstcv ON wstcv.Name = rod.WaterSourceTypeCV
         GROUP BY rud.ReportingUnitID
     ) o
         INNER JOIN Core.ReportingUnits_dim rud ON o.ReportingUnitID = rud.ReportingUnitID
    GO


DROP TYPE [Core].[RegulatoryOverlayTableType]
DROP TYPE [Core].[RegulatoryReportingUnitsTableType]
