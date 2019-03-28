using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class WaterAllocation
    {
        public string OrganizationUUID { get; set; }
        public string AllocationUUID { get; set; }
        public string AllocationNativeID { get; set; }
        public string AllocationOwner { get; set; }
        public string AllocationBasisCV { get; set; }
        public string AllocationLegalStatusCodeCV { get; set; }
        public DateTime? AllocationApplicationDate { get; set; }
        public DateTime? WaterAllocationPriorityDate { get; set; }
        public DateTime? AllocationExpirationDate { get; set; }
        public string AllocationChangeApplicationIndicator { get; set; }
        public string SiteUUID { get; set; }
        public string VariableSpecificUUID { get; set; }
        public string BeneficialUseCategory { get; set; }
        public string PrimaryUseCategory { get; set; }
        public string WaterSourceUUID { get; set; }
        public string MethodUUID { get; set; }
        public DateTime? TimeframeStartDate { get; set; }
        public DateTime? TimeframeEndDate { get; set; }
        public DateTime? DataPublicationDate { get; set; }
        public string ReportYear { get; set; }
        public double? AllocationCropDutyAmount { get; set; }
        public double? AllocationAmount { get; set; }
        public double? AllocationMaximum { get; set; }
        public double? PopulationServed { get; set; }
        public double? PowerGeneratedGWh { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string AllocationCommunityWaterSupplySystem { get; set; }
        public string SDWISIdentifier { get; set; }
        public string Geometry { get; set; }
    }
}
