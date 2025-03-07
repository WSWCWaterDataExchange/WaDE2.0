using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

public class TimeSeriesSearchItem
{
   public string SiteVariableAmountId { get; set; }
   public Organization Organization { get; set; }
   public VariableSpecific VariableSpecific { get; set; }
   public WaterSourceSummary WaterSource { get; set; }
   public Method Method { get; set; }
   public DateTime? TimeframeStart { get; set; }
   public DateTime? TimeframeEnd { get; set; }
   public string ReportYear { get; set; }
   public double? Amount { get; set; }
   public long? PopulationServed { get; set; }
   public double? PowerGeneratedGWh { get; set; }
   public double? IrrigatedAcreage { get; set; }
   public string IrrigationMethod { get; set; }
   public string CropType { get; set; }
   public string CommunityWaterSupplySystem { get; set; }
   public string SdwisIdentifier { get; set; }
   public string AssociatedNativeAllocationIDs { get; set; } 
   public string CustomerType { get; set; }
   public double? AllocationCropDutyAmount { get; set; }
   public string PrimaryUseCategoryCv { get; set; }
   public string PrimaryUseCategoryWaDEName { get; set; }
   public string PowerType { get; set; }
   public Site Site { get; set; }
}