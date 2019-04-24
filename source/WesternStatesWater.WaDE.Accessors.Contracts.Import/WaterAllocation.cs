using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class WaterAllocation
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string AllocationUUID { get; set; }

        [NullValues("")]
        public string AllocationNativeID { get; set; }

        [NullValues("")]
        public string AllocationOwner { get; set; }

        [NullValues("")]
        public string AllocationBasisCV { get; set; }

        [NullValues("")]
        public string AllocationLegalStatusCodeCV { get; set; }

        public DateTime? AllocationApplicationDate { get; set; }

        public DateTime? WaterAllocationPriorityDate { get; set; }

        public DateTime? AllocationExpirationDate { get; set; }

        [NullValues("")]
        public string AllocationChangeApplicationIndicator { get; set; }

        [NullValues("")]
        public string SiteUUID { get; set; }

        [NullValues("")]
        public string VariableSpecificUUID { get; set; }

        [NullValues("")]
        public string BeneficialUseCategory { get; set; }

        [NullValues("")]
        public string PrimaryUseCategory { get; set; }

        [NullValues("")]
        public string WaterSourceUUID { get; set; }

        [NullValues("")]
        public string MethodUUID { get; set; }

        public DateTime? TimeframeStartDate { get; set; }

        public DateTime? TimeframeEndDate { get; set; }

        public DateTime? DataPublicationDate { get; set; }

        [NullValues("")]
        public string ReportYear { get; set; }

        public double? AllocationCropDutyAmount { get; set; }

        public double? AllocationAmount { get; set; }

        public double? AllocationMaximum { get; set; }

        public double? PopulationServed { get; set; }

        public double? PowerGeneratedGWh { get; set; }

        public double? IrrigatedAcreage { get; set; }

        [NullValues("")]
        public string AllocationCommunityWaterSupplySystem { get; set; }

        [NullValues("")]
        public string SDWISIdentifier { get; set; }

        [NullValues("")]
        public string Latitude { get; set; }

        [NullValues("")]
        public string Longitude { get; set; }
    }
}
