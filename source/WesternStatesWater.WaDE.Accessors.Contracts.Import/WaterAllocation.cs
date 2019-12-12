using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class WaterAllocation
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string VariableSpecificUUID { get; set; }

        [NullValues("")]
        public string SiteUUID { get; set; }

        [NullValues("")]
        public string WaterSourceUUID { get; set; }

        [NullValues("")]
        public string MethodUUID { get; set; }

        [NullValues("")]
        public string BeneficialUseCategory { get; set; }
        
        [NullValues("")]
        public string PrimaryUseCategory { get; set; }

        public DateTime? DataPublicationDate { get; set; }

        [NullValues("")]
        public string DataPublicationDOI { get; set; }

        [NullValues("")]
        public string AllocationNativeID { get; set; }

        public DateTime? AllocationApplicationDate { get; set; }

        public DateTime? AllocationPriorityDate { get; set; }

        public DateTime? AllocationExpirationDate { get; set; }

        [NullValues("")]
        public string AllocationOwner { get; set; }

        [NullValues("")]
        public string AllocationBasisCV { get; set; }

        [NullValues("")]
        public string AllocationLegalStatusCV { get; set; }

        [NullValues("")]
        public string AllocationTypeCV { get; set; }

        public DateTime? AllocationTimeframeStart { get; set; }

        public DateTime? AllocationTimeframeEnd { get; set; }

        public string AllocationCropDutyAmount { get; set; }

        public string AllocationAmount { get; set; }

        public string AllocationMaximum { get; set; }

        public string PopulationServed { get; set; }

        public string GeneratedPowerCapacityMW { get; set; }

        public string IrrigatedAcreage { get; set; }

        [NullValues("")]
        public string AllocationCommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string AllocationSDWISIdentifierCV { get; set; }

        [NullValues("")]
        public string AllocationAssociatedWithdrawalSiteIDs { get; set; }

        [NullValues("")]
        public string AllocationAssociatedConsumptiveUseSiteIDs { get; set; }

        [NullValues("")]
        public string AllocationChangeApplicationIndicator { get; set; }

        [NullValues("")]
        public string LegacyAllocationIDs { get; set; }

        [NullValues("")]
        public string CustomerTypeCV { get; set; }

        [NullValues("")]
        public string IrrigationMethodCV { get; set; }

        [NullValues("")]
        public string CropTypeCV { get; set; }

        [NullValues("")]
        public string WaterAllocationNativeURL { get; set; }

        [NullValues("")]
        public string CommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string PowerType { get; set; }
    }
}
