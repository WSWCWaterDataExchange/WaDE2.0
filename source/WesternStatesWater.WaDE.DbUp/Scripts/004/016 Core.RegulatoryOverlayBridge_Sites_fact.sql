CREATE TABLE [Core].[RegulatoryOverlayBridge_Sites_fact](
    [RegulatoryOverlayBridgeID] [bigint] IDENTITY(1,1) NOT NULL,
    [RegulatoryOverlayID] [bigint] NOT NULL,
    [SiteID] [bigint] NOT NULL,
    CONSTRAINT [pkRegulatoryOverlayBridge_Sites_fact] PRIMARY KEY CLUSTERED
(
[RegulatoryOverlayBridgeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [Core].[RegulatoryOverlayBridge_Sites_fact] WITH CHECK ADD CONSTRAINT [fk_RegulatoryOverlayBridge_Sites_fact_RegulatoryOverlay_dim] FOREIGN KEY([RegulatoryOverlayID])
    REFERENCES [Core].[RegulatoryOverlay_dim] ([RegulatoryOverlayID])

ALTER TABLE [Core].[RegulatoryOverlayBridge_Sites_fact] CHECK CONSTRAINT [fk_RegulatoryOverlayBridge_Sites_fact_RegulatoryOverlay_dim]

ALTER TABLE [Core].[RegulatoryOverlayBridge_Sites_fact] WITH CHECK ADD CONSTRAINT [fk_RegulatoryOverlayBridge_Sites_fact_Sites_dim] FOREIGN KEY([SiteID])
    REFERENCES [Core].[Sites_dim] ([SiteID])

ALTER TABLE [Core].[RegulatoryOverlayBridge_Sites_fact] CHECK CONSTRAINT [fk_RegulatoryOverlayBridge_Sites_fact_Sites_dim]

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
    [RegulatoryOverlayUUDIs] [nvarchar](max) NULL 
    )
GO

EXEC Core.UpdateUUDT 'Core', 'SiteTableType';
GO
