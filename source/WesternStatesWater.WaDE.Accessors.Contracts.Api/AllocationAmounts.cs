using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class AllocationAmounts
    {
        public Organization Organization { get; set; }
        public Allocation Allocation { get; set; }
        public Site Site { get; set; }
        public VariableSpecific VariableSpecific { get; set; }
        public List<BeneficialUse> BeneficialUses { get; set; }
        public BeneficialUse PrimaryBeneficialUse { get; set; }
        public WaterSource WaterSource { get; set; }
        public Method Method { get; set; }
        public DateTime TimeframeStart { get; set; }
        public DateTime TimeframeEnd { get; set; }
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
        public string InterbasinTransferToID { get; set; }
        public string InterbasinTransferFromID { get; set; }
        public string Geometry { get; set; }
    }
}
