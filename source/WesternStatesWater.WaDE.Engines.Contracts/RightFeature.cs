using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class RightFeature : FeatureBase
{
    [JsonPropertyName("nId")]
    public string AllocationNativeID { get; set; }
    [JsonPropertyName("owner")]
    public string AllocationOwner { get; set; }
    [JsonPropertyName("appDate")]
    public DateTime? AllocationApplicationDate { get; set; }
    [JsonPropertyName("priorityDate")]
    public DateTime? AllocationPriorityDate { get; set; }
    [JsonPropertyName("legalStatus")]
    public string AllocationLegalStatusCodeCV { get; set; }
    [JsonPropertyName("expires")]
    public DateTime? AllocationExpirationDate { get; set; }
    [JsonPropertyName("acreage")]
    public double? AllocationAcreage { get; set; }
    [JsonPropertyName("basis")]
    public string AllocationBasisCV { get; set; }
    [JsonPropertyName("start")]
    public string TimeframeStart { get; set; }
    [JsonPropertyName("end")]
    public string TimeframeEnd { get; set; }
    [JsonPropertyName("cropDutyAmt")]
    public double? AllocationCropDutyAmount { get; set; }
    [JsonPropertyName("cfs")]
    public double? AllocationFlow_CFS { get; set; }
    [JsonPropertyName("af")]
    public double? AllocationVolume_AF { get; set; }
    [JsonPropertyName("popServed")]
    public long? PopulationServed { get; set; }
    [JsonPropertyName("generatedPowerCap")]
    public double? GeneratedPowerCapacityMW { get; set; }
    [JsonPropertyName("commWaterSupply")]
    public string AllocationCommunityWaterSupplySystem { get; set; }
    [JsonPropertyName("sdwis")]
    public string AllocationSDWISIdentifier { get; set; }
    [JsonPropertyName("varType")]
    public string VariableSpecificTypeCV { get; set; }
    [JsonPropertyName("beneficialUses")]
    public List<string> BeneficialUses { get; set; }
    [JsonPropertyName("exmptVolFlowPriority")]
    public bool ExemptOfVolumeFlowPriority { get; set; }
    [JsonPropertyName("states")]
    public List<string> States { get; set; }
}