create index IX_POUSiteID on core.PODSite_POUSite_fact (POUSiteId)
create index IX_PODSiteID on core.PODSite_POUSite_fact (PODSiteId)
create unique index AK_AllocationAmountID_BeneficialUsesCV ON core.AllocationBridge_BeneficialUses_fact