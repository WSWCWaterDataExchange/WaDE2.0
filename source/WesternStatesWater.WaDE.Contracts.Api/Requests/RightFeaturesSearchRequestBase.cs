namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public abstract class RightFeaturesSearchRequestBase : FeaturesSearchRequestBase
{
    public string AllocationUuids { get; set; }
    public string SiteUuids { get; set; }
    public string States { get; set; }
    public string WaterSourceTypes { get; set; }
    public string BeneficialUses { get; set; }
    public string OwnerClassificationTypes { get; set; }
}