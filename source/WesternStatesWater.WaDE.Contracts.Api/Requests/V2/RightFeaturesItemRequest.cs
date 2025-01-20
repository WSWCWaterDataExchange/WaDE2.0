namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class RightFeaturesItemRequest : RightFeaturesSearchRequestBase
{
    public string AllocationUuids { get; set; }
    public string SiteUuids { get; set; }
    public string States { get; set; }
}