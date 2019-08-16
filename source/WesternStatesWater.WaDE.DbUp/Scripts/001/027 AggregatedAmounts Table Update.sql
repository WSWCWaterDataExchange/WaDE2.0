--add CommunityWaterSupplySystem and AllocationCropDutyAmount columns to Core.AggregatedAmounts_fact
ALTER TABLE Core.AggregatedAmounts_fact
add CommunityWaterSupplySystem NVARCHAR(250) NULL

ALTER TABLE Core.AggregatedAmounts_fact
add AllocationCropDutyAmount float NULL



