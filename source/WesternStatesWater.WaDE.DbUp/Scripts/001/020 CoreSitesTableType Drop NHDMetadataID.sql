CREATE TYPE [Core].[SiteTableType_new] AS TABLE(
	[SiteUUID] [nvarchar](55) NULL,
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
	[EPSGCodeCV] [nvarchar](50) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'SiteTableType';
GO
