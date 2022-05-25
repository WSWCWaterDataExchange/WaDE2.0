CREATE TABLE [Core].[WaterSourceBridge_Sites_fact](
	[WaterSourceBridgeSiteID] [bigint] IDENTITY(1,1) NOT NULL,
	[WaterSourceID] [bigint] NOT NULL,
	[SiteID] [bigint] NOT NULL,
    CONSTRAINT [PK_WaterSourceBridge_Sites_fact] PRIMARY KEY CLUSTERED ([WaterSourceBridgeSiteID] ASC)
);
GO

INSERT INTO Core.WaterSourceBridge_Sites_fact (WaterSourceID, SiteID)
  SELECT s.WaterSourceId, s.SiteId
    FROM Core.Sites_dim s
	WHERE s.WaterSourceID is not null

ALTER TABLE [Core].[WaterSourceBridge_Sites_fact]  WITH CHECK ADD CONSTRAINT [FK_Sites] FOREIGN KEY([SiteID])
  REFERENCES [Core].[Sites_dim] ([SiteID]);
GO

ALTER TABLE [Core].[WaterSourceBridge_Sites_fact]  WITH CHECK ADD CONSTRAINT [FK_WaterSource] FOREIGN KEY([WaterSourceID])
  REFERENCES [Core].[WaterSources_dim] ([WaterSourceID]);
GO

ALTER TABLE Core.Sites_dim DROP CONSTRAINT fk_Sites_dim_WaterSource_dim;
GO

ALTER TABLE Core.Sites_dim DROP COLUMN WaterSourceID;
GO