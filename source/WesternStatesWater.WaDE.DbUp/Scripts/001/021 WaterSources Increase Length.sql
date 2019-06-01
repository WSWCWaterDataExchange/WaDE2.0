ALTER TABLE Core.WaterSources_dim
ALTER COLUMN WaterSourceUUID NVARCHAR(250) NOT NULL
GO

CREATE TYPE [Core].[WaterSourceTableType_new] AS TABLE(
	[WaterSourceUUID] [nvarchar](250) NULL,
	[WaterSourceNativeID] [nvarchar](250) NULL,
	[WaterSourceName] [nvarchar](250) NULL,
	[WaterSourceTypeCV] [nvarchar](100) NULL,
	[WaterQualityIndicatorCV] [nvarchar](100) NULL,
	[GNISFeatureNameCV] [nvarchar](250) NULL,
	[Geometry] [nvarchar](max) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'WaterSourceTableType';
GO
