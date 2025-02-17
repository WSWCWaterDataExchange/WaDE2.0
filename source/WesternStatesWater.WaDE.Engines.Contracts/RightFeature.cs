namespace WesternStatesWater.WaDE.Engines.Contracts;

public class RightFeature : FeatureBase
{
    public string AllocationUuid { get; set; }
    public string? AllocationNativeId { get; set; }
    public string? AllocationOwner { get; set; }
    public string? AllocationTypeCv { get; set; }
    public string? AllocationTypeWaDEName { get; set; }
    public DateTime? AllocationApplicationDate { get; set; }
    public DateTime? AllocationPriorityDate { get; set; }
    public string? AllocationLegalStatusCv { get; set; }
    public string? AllocationLegalStatusWaDEName { get; set; }
    public DateTime? AllocationExpirationDate { get; set; }
    public string? AllocationChangeApplicationIndicator { get; set; }
    public string? LegacyAllocationIDs { get; set; }
    public double? IrrigatedAcreage { get; set; }
    public string? AllocationBasisCv { get; set; }
    public string? AllocationTimeframeStart { get; set; }
    public string? AllocationTimeframeEnd { get; set; }
    public DateTime? DataPublicationDate { get; set; }
    public double? AllocationCropDutyAmount { get; set; }
    public double? AllocationFlow_CFS { get; set; }
    public double? AllocationVolume_AF { get; set; }
    public long? PopulationServed { get; set; }
    public double? GeneratedPowerCapacityMW { get; set; }
    public string? AllocationCommunityWaterSupplySystem { get; set; }
    public string? SdwisIdentifierCv { get; set; }
    public Method? Method { get; set; }
    public VariableSpecific? VariableSpecific { get; set; }
    public List<BeneficialUse>? BeneficialUses { get; set; }
    public Organization? Organization { get; set; }
    public bool? ExemptOfVolumeFlowPriority { get; set; }
}