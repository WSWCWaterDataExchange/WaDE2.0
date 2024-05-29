SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
        sd.RowNumber
         ,RegulatoryOverlayID = rodim.RegulatoryOverlayID
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

    --wire up the foreign keys
    SELECT
        sd.*
         ,wt.WaterSourceID
    INTO
        #TempJoinedSiteData
    FROM
        #TempSiteData sd
            LEFT OUTER JOIN Core.WaterSources_dim wt ON sd.WaterSourceUUID = wt.WaterSourceUUID;

    --data validation
    WITH q1 AS
             (
                 SELECT 'SiteUUID Not Valid' Reason, *
                 FROM #TempJoinedSiteData
                 WHERE SiteUUID IS NULL
                 UNION ALL
                 SELECT 'SiteName Not Valid' Reason, *
                 FROM #TempJoinedSiteData
                 WHERE SiteName IS NULL
                 UNION ALL
                 SELECT 'CoordinateMethodCV Not Valid' Reason, *
                 FROM #TempJoinedSiteData
                 WHERE CoordinateMethodCV IS NULL
                 UNION ALL
                 SELECT 'EPSGCodeCV Not Valid' Reason, *
                 FROM #TempJoinedSiteData
                 WHERE EPSGCodeCV IS NULL
                 UNION ALL
                 SELECT 'WaterSourceID Not Valid' Reason, *
                 FROM #TempJoinedSiteData
                 WHERE WaterSourceID IS NULL and WaterSourceUUID is not null and Len(WaterSourceUUID) > 0
                 UNION ALL
                 SELECT 'PODorPOUSite found POD record with missing longitude and latitude' Reason, *
                 FROM #TempJoinedSiteData
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
    MERGE INTO Core.Sites_dim AS Target USING #TempJoinedSiteData AS Source
        ON Target.SiteUUID = Source.SiteUUID
        WHEN MATCHED THEN
            UPDATE SET
                SiteNativeID = Source.SiteNativeID
                ,SiteName = Source.SiteName
                ,USGSSiteID = Source.USGSSiteID
                ,SiteTypeCV = Source.SiteTypeCV
                ,WaterSourceID = Source.WaterSourceID
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
                ,WellDepth = Source.WellDepth
                
        WHEN NOT MATCHED THEN
            INSERT
                (SiteUUID
                ,SiteNativeID
                ,SiteName
                ,USGSSiteID
                ,SiteTypeCV
                ,WaterSourceID
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
                ,WellDepth
                )
            VALUES
                (Source.SiteUUID
                ,Source.SiteNativeID
                ,Source.SiteName
                ,Source.USGSSiteID
                ,Source.SiteTypeCV
                ,Source.WaterSourceID
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
                ,Source.WellDepth
                )
            OUTPUT
                inserted.SiteID
                ,Source.RowNumber
            INTO
                #TempSiteRecords;

    -- insert into bridge
    CREATE TABLE #TempBridge (RegulatoryOverlayID BIGINT, SiteID BIGINT);

    INSERT INTO #TempBridge (RegulatoryOverlayID, SiteID)
    SELECT DISTINCT
        ro.RegulatoryOverlayID
                  ,tsr.SiteID
    FROM
        #TempSiteRecords tsr
            INNER JOIN #TempRegulatorOverlayBridgeData ro ON ro.RowNumber = tsr.RowNumber;

    WITH q2 as (SELECT * FROM Core.RegulatoryOverlayBridge_Sites_fact robsf WHERE robsf.SiteID IN (SELECT SiteID FROM #TempSiteRecords))
        MERGE INTO q2 AS Target USING #TempBridge AS Source
    ON Target.RegulatoryOverlayID = Source.RegulatoryOverlayID AND
        Target.SiteID = Source.SiteID

        WHEN NOT MATCHED THEN
    INSERT
    (SiteID, RegulatoryOverlayID)
    VALUES
        (Source.SiteID, Source.RegulatoryOverlayID)

        WHEN NOT MATCHED BY SOURCE THEN
    DELETE;

    RETURN 0;
END
GO
