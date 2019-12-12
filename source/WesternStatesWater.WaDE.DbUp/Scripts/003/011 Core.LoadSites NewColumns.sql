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
	[County] [nvarchar](20) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'SiteTableType';
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
            );
    RETURN 0;
END