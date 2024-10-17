CREATE NONCLUSTERED INDEX IX_WaterSourceBridge_Sites_WaterSourceID
ON [Core].[WaterSourceBridge_Sites_fact] ([WaterSourceID])
GO

CREATE NONCLUSTERED INDEX IX_AggregatedAmounts_WaterSourceID
ON [Core].[AggregatedAmounts_fact] ([WaterSourceID])
GO

CREATE NONCLUSTERED INDEX IX_SiteVariableAmounts_WaterSourceID
ON [Core].[SiteVariableAmounts_fact] ([WaterSourceID])
GO