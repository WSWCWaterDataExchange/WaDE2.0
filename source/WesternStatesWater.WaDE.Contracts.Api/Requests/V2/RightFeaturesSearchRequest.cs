namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class RightFeaturesSearchRequest : FeaturesSearchRequestBase
{
    public string AllocationUuids { get; set; }
    public string SiteUuids { get; set; }
}