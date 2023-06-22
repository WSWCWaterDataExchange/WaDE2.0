CREATE NONCLUSTERED INDEX IX_WaterSourceBridge_Sites_SiteID
ON [Core].[WaterSourceBridge_Sites_fact] ([SiteID])
INCLUDE ([WaterSourceID])
GO

CREATE NONCLUSTERED INDEX IX_WaterSourceBridge_Sites_SiteID
ON [Core].[WaterSourceBridge_Sites_fact] ([WaterSourceID])
INCLUDE ([SiteID])
GO

CREATE NONCLUSTERED INDEX IX_AllocationAmounts_AllocationPriorityDateID
ON [Core].[AllocationAmounts_fact] ([AllocationPriorityDateID])
GO