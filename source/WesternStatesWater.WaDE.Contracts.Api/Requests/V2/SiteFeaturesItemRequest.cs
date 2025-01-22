namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesItemRequest : SiteFeaturesSearchRequestBase
{
    public string SiteTypes { get; set; }
    public string States { get; set; }
    public string WaterSourceTypes { get; set; }
    public string SiteUuids { get; set; }
    public string OverlayUuids { get; set; }
    public string AllocationUuids { get; set; }
}