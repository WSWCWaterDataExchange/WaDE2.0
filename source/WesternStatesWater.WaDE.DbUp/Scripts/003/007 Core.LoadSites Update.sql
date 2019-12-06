USE [Wade2.0]
GO
/****** Object:  StoredProcedure [Core].[LoadSites]    Script Date: 12/6/2019 9:13:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [Core].[LoadSites]    Script Date: 5/2/2019 11:13:33 AM ******/
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

    --data validation
    WITH q1 AS
    (
        SELECT 'WaDESiteUUID Not Valid' Reason, *
        FROM #TempSiteData
        WHERE WaDESiteUUID IS NULL
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
    )
    SELECT * INTO #TempErrorSiteRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorSiteRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('Sites', @RunId, (SELECT * FROM #TempErrorSiteRecords FOR JSON PATH));
        RETURN 1;
    END

    --merge the data
    MERGE INTO Core.Sites_dim AS Target USING #TempSiteData AS Source
		ON Target.SiteUUID = Source.WaDESiteUUID
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
            )
        VALUES
            (Source.WaDESiteUUID
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
            );
    RETURN 0;
END