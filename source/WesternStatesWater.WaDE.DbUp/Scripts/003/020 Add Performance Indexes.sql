DELETE FROM Core.AllocationBridge_BeneficialUses_fact WHERE NOT AllocationBridgeId IN (select min(AllocationBridgeId) from Core.AllocationBridge_BeneficialUses_fact group by BeneficialUseCV, AllocationAmountId);
DELETE FROM Core.AllocationBridge_Sites_fact WHERE NOT AllocationBridgeId IN (select min(AllocationBridgeId) from Core.AllocationBridge_Sites_fact group by SiteId, AllocationAmountId);

CREATE UNIQUE INDEX  AK_Date_Date                                                        on Core.Date_dim                             (Date);
CREATE UNIQUE INDEX  AK_Site_SiteUuid                                                    on Core.Sites_dim                            (SiteUuid);
CREATE SPATIAL INDEX IX_Sites_Geometry                                                   on Core.Sites_dim                            (Geometry) WITH (BOUNDING_BOX = (xmin=-180, ymin=-90, xmax=180, ymax=90));
CREATE SPATIAL INDEX IX_Sites_SitePoint                                                  on Core.Sites_dim                            (SitePoint) WITH (BOUNDING_BOX = (xmin=-180, ymin=-90, xmax=180, ymax=90));
CREATE INDEX         IX_Sites_SiteType                                                   on Core.Sites_dim                            (SiteTypeCv);
CREATE INDEX         IX_Sites_Huc8                                                       on Core.Sites_dim                            (HUC8);
CREATE INDEX         IX_Sites_Huc12                                                      on Core.Sites_dim                            (HUC12);
CREATE INDEX         IX_Sites_County                                                     on Core.Sites_dim                            (County);

CREATE INDEX         IX_BeneficialUses_UsgsCategory                                      on CVs.BeneficialUses                        (UsgsCategory);
CREATE INDEX         IX_Variables_Variable                                               on Core.Variables_dim                        (VariableCV);
CREATE INDEX         IX_Variables_VariableSpecific                                       on Core.Variables_dim                        (VariableSpecificCV);
CREATE INDEX         IX_ReportingUnits_ReportingUnitUuid                                 on Core.ReportingUnits_dim                   (ReportingUnitUuid);
CREATE INDEX         IX_ReportingUnits_ReportingUnitTypeCv                               on Core.ReportingUnits_dim                   (ReportingUnitTypeCV);
CREATE SPATIAL INDEX IX_ReportingUnits_Geometry                                          on Core.ReportingUnits_dim                   (Geometry) WITH (BOUNDING_BOX = (xmin=-180, ymin=-90, xmax=180, ymax=90));
CREATE UNIQUE INDEX  AK_Organizations_OrganizationUuid                                   on Core.Organizations_dim                    (OrganizationUuid);
CREATE INDEX         IX_Organizations_State                                              on Core.Organizations_dim                    (State);

CREATE INDEX         IX_AllocationAmounts_AllocationPriorityDate                         on Core.AllocationAmounts_fact               (AllocationPriorityDateID);
CREATE INDEX         IX_AllocationAmounts_PrimaryUseCategory                             on Core.AllocationAmounts_fact               (PrimaryUseCategoryCV);
CREATE UNIQUE INDEX  AK_AllocationBridgeBeneficialUses_BeneficialUseCvAllocationAmountId on Core.AllocationBridge_BeneficialUses_fact (BeneficialUseCV, AllocationAmountId);
CREATE INDEX         IX_AllocationBridgeBeneficialUses_AllocationAmountId                on Core.AllocationBridge_BeneficialUses_fact (AllocationAmountId);
CREATE UNIQUE INDEX  AK_AllocationBridgeSites_SiteIdAllocationAmountId                   on Core.AllocationBridge_Sites_fact          (SiteId, AllocationAmountId);

CREATE INDEX         IX_SiteVariableAmounts_TimeframeStartDate                           on Core.SiteVariableAmounts_fact             (TimeframeStartId);
CREATE INDEX         IX_SiteVariableAmounts_TimeframeEndDate                             on Core.SiteVariableAmounts_fact             (TimeframeEndId);
CREATE INDEX         IX_SiteVariableAmounts_VariableSpecificId                           on Core.SiteVariableAmounts_fact             (VariableSpecificId);
CREATE INDEX         IX_SiteVariableAmounts_PrimaryUseCategory                           on Core.SiteVariableAmounts_fact             (PrimaryUseCategoryCV);
CREATE INDEX         IX_SiteVariableAmounts_SiteID                                       on Core.SiteVariableAmounts_fact             (SiteId);
CREATE UNIQUE INDEX  AK_SiteBridgeBeneficialUses_BeneficialUseCvSiteVariableAmountId     on Core.SitesBridge_BeneficialUses_fact      (BeneficialUseCV, SiteVariableAmountID);
CREATE INDEX         IX_SiteBridgeBeneficialUses_SiteVariableAmountId                    on Core.SitesBridge_BeneficialUses_fact      (SiteVariableAmountID);

CREATE INDEX         IX_AggregatedAmounts_TimeframeStartDate                             on Core.AggregatedAmounts_fact               (TimeframeStartId);
CREATE INDEX         IX_AggregatedAmounts_TimeframeEndDate                               on Core.AggregatedAmounts_fact               (TimeframeEndId);
CREATE INDEX         IX_AggregatedAmounts_VariableSpecificId                             on Core.AggregatedAmounts_fact               (VariableSpecificId);
CREATE INDEX         IX_AggregatedAmounts_ReportingUnitId                                on Core.AggregatedAmounts_fact               (ReportingUnitId);
CREATE INDEX         IX_AggregatedAmounts_PrimaryUseCategory                             on Core.AggregatedAmounts_fact               (PrimaryUseCategoryCv);
CREATE UNIQUE INDEX  AK_SiteBridgeBeneficialUses_BeneficialUseCvSiteVariableAmountId     on Core.AggBridge_BeneficialUses_fact        (BeneficialUseCV, AggregatedAmountId);
CREATE INDEX         IX_SiteBridgeBeneficialUses_AggregatedAmountId                      on Core.AggBridge_BeneficialUses_fact        (AggregatedAmountId);

CREATE UNIQUE INDEX  IX_RegulatoryOverlay_OverlayUuid                                    on Core.RegulatoryOverlay_dim                (RegulatoryOverlayUuid);
CREATE INDEX         IX_RegulatoryOverlay_StatutoryEffectiveDate                         on Core.RegulatoryOverlay_dim                (StatutoryEffectiveDate);
CREATE INDEX         IX_RegulatoryOverlay_StatutoryEndDate                               on Core.RegulatoryOverlay_dim                (StatutoryEndDate);
CREATE INDEX         IX_RegulatoryOverlay_RegulatoryStatus                               on Core.RegulatoryOverlay_dim                (RegulatoryStatusCv);
CREATE INDEX         IX_RegulatoryReportingUnits_OrganizationId                          on Core.RegulatoryReportingUnits_fact        (OrganizationId);
CREATE INDEX         IX_RegulatoryReportingUnits_RegulatoryOverlayId                     on Core.RegulatoryReportingUnits_fact        (RegulatoryOverlayId);
CREATE INDEX         IX_RegulatoryReportingUnits_ReportingUnitId                         on Core.RegulatoryReportingUnits_fact        (ReportingUnitId);