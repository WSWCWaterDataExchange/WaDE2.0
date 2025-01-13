using WesternStatesWater.WaDE.Engines.Contracts.Attributes;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class RightFeature : FeatureBase
{
    [FeaturePropertyName("nId")]
    public string AllocationNativeID { get; set; }
    [FeaturePropertyName("owner")]
    public string AllocationOwner { get; set; }
    [FeaturePropertyName("appDate")]
    public DateTime? AllocationApplicationDate { get; set; }
    [FeaturePropertyName("priorityDate")]
    public DateTime? AllocationPriorityDate { get; set; }
    [FeaturePropertyName("legalStatus")]
    public string AllocationLegalStatusCodeCV { get; set; }
    [FeaturePropertyName("expires")]
    public DateTime? AllocationExpirationDate { get; set; }
    [FeaturePropertyName("acreage")]
    public double? AllocationAcreage { get; set; }
    [FeaturePropertyName("basis")]
    public string AllocationBasisCV { get; set; }
    [FeaturePropertyName("start")]
    public string TimeframeStart { get; set; }
    [FeaturePropertyName("end")]
    public string TimeframeEnd { get; set; }
    [FeaturePropertyName("cropDutyAmt")]
    public double? AllocationCropDutyAmount { get; set; }
    [FeaturePropertyName("cfs")]
    public double? AllocationFlow_CFS { get; set; }
    [FeaturePropertyName("af")]
    public double? AllocationVolume_AF { get; set; }
    [FeaturePropertyName("popServed")]
    public long? PopulationServed { get; set; }
    [FeaturePropertyName("generatedPowerCap")]
    public double? GeneratedPowerCapacityMW { get; set; }
    [FeaturePropertyName("commWaterSupply")]
    public string AllocationCommunityWaterSupplySystem { get; set; }
    [FeaturePropertyName("sdwis")]
    public string AllocationSDWISIdentifier { get; set; }
    [FeaturePropertyName("varType")]
    public string VariableSpecificTypeCV { get; set; }
    [FeaturePropertyName("beneficialUses")]
    public List<string> BeneficialUses { get; set; }
    [FeaturePropertyName("exmptVolFlowPriority")]
    public bool ExemptOfVolumeFlowPriority { get; set; }
}