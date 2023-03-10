using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class SiteVariableAmount
    {
        public string WaterSourceUUID { get; set; }
        public string AllocationGNISIDCV { get; set; }
        public DateTime? TimeframeStart { get; set; }
        public DateTime? TimeframeEnd { get; set; }
        public DateTime? DataPublicationDate { get; set; }
        public double? AllocationCropDutyAmount { get; set; }
        public double? Amount { get; set; }
        public string IrrigationMethodCV { get; set; }
        public double? IrrigatedAcreage { get; set; }
        public string CropTypeCV { get; set; }
        public long? PopulationServed { get; set; }
        public double? PowerGeneratedGWh { get; set; }
        public string AllocationCommunityWaterSupplySystem { get; set; }
        public string SDWISIdentifier { get; set; }
        public string DataPublicationDOI { get; set; }
        public string ReportYearCV { get; set; }
        public string MethodUUID { get; set; }
        public string VariableSpecificUUID { get; set; }
        public string SiteUUID { get; set; }
        public string AssociatedNativeAllocationIDs { get; set; }
        public List<string> BeneficialUses { get; set; }
    }
}
