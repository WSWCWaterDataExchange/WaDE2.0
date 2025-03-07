using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class TimeSeriesFeature : FeatureBase
{
    [JsonPropertyName("organization")]
    public Organization Organization { get; set; }
    [JsonPropertyName("variableSpecific")]
    public VariableSpecific VariableSpecific { get; set; }
    [JsonPropertyName("waterSource")]
    public WaterSourceSummary WaterSource { get; set; }
    [JsonPropertyName("method")]
    public Method Method { get; set; }
    [JsonPropertyName("timeframeStart")]
    public DateTime? TimeframeStart { get; set; }
    [JsonPropertyName("tiemframeEnd")]
    public DateTime? TimeframeEnd { get; set; }
    [JsonPropertyName("reportYear")]
    public string ReportYear { get; set; }
    [JsonPropertyName("amt")]
    public double? Amount { get; set; }
    [JsonPropertyName("populationServed")]
    public long? PopulationServed { get; set; }
    [JsonPropertyName("powerGeneratedGWh")]
    public double? PowerGeneratedGWh { get; set; }
    [JsonPropertyName("irrigatedAcreage")]
    public double? IrrigatedAcreage { get; set; }
    [JsonPropertyName("irrigationMethod")]
    public string IrrigationMethod { get; set; }
    [JsonPropertyName("cropType")]
    public string CropType { get; set; }
    [JsonPropertyName("communityWaterSupplySystem")]
    public string CommunityWaterSupplySystem { get; set; }
    [JsonPropertyName("sdwisIdentifier")]
    public string SdwisIdentifier { get; set; }
    [JsonPropertyName("associatedNativeAllocationIds")]
    public string AssociatedNativeAllocationIDs { get; set; }
    [JsonPropertyName("customerType")]
    public string CustomerType { get; set; }
    [JsonPropertyName("allocationCropDutyAmt")]
    public double? AllocationCropDutyAmount { get; set; }
    [JsonPropertyName("primaryUse")]
    public string PrimaryUseCategoryCv { get; set; }
    [JsonPropertyName("primaryUseWaDEName")]
    public string PrimaryUseCategoryWaDEName { get; set; }
    [JsonPropertyName("powerType")]
    public string PowerType { get; set; }
    [JsonPropertyName("site")]
    public SiteFeature Site { get; set; }
}