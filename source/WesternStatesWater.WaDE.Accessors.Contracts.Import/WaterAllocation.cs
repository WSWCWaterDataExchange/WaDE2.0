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

        public double? AllocationCropDutyAmount { get; set; }

        public double? AllocationAmount { get; set; }

        public double? AllocationMaximum { get; set; }

        public double? PopulationServed { get; set; }

        public double? PowerGeneratedGWh { get; set; }

        public double? IrrigatedAcreage { get; set; }

        [NullValues("")]
        public string AllocationCommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string AllocationSDWISIdentifier { get; set; }

        [NullValues("")]
        public string AllocationAssociatedWithdrawalSiteIDs { get; set; }

        [NullValues("")]
        public string AllocationAssociatedConsumptiveUseSiteIDs { get; set; }

        [NullValues("")]
        public string AllocationChangeApplicationIndicator { get; set; }

        [NullValues("")]
        public string LegacyAllocationIDs { get; set; }
    }
}
