ALTER TABLE Core.SitesVariableAmountBridgeAllocations_fact DROP CONSTRAINT FK_SitesVariableAmountBridgeAllocations_fact_SiteVariableAmounts_fact

ALTER TABLE Core.SitesVariableAmountBridgeAllocations_fact DROP CONSTRAINT FK_SitesVariableAmountBridgeAllocations_fact_Allocations_dim

ALTER TABLE Core.SitesAllocationAmountsBridge_fact DROP CONSTRAINT FK_SitesAllocationAmountsBridge_fact_Sites_dim

ALTER TABLE Core.SitesAllocationAmountsBridge_fact DROP CONSTRAINT FK_SitesAllocationAmountsBridge_fact_AllocationAmounts_fact

ALTER TABLE Core.NHDMetadata DROP CONSTRAINT fk_NHDMetadata_NHDProduct

ALTER TABLE Core.NHDMetadata DROP CONSTRAINT fk_NHDMetadata_NHDNetworkStatus

ALTER TABLE Core.Allocations_dim DROP CONSTRAINT FK_AllocationsDate_PriorityDate

ALTER TABLE Core.Allocations_dim DROP CONSTRAINT FK_AllocationsDate_ExpirationDate

ALTER TABLE Core.Allocations_dim DROP CONSTRAINT FK_AllocationsDate_ApplicationDate

ALTER TABLE Core.Sites_dim DROP CONSTRAINT fk_Sites_NHDMetadata

ALTER TABLE Core.Sites_dim DROP COLUMN NHDMetadataID

DROP TABLE Core.SitesVariableAmountBridgeAllocations_fact

DROP TABLE Core.SitesAllocationAmountsBridge_fact

DROP TABLE Core.NHDMetadata

DROP TABLE Core.Allocations_dim_Input

DROP TABLE Core.Allocations_dim
