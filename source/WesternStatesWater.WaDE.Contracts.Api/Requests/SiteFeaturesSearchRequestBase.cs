namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public abstract class SiteFeaturesSearchRequestBase : FeaturesSearchRequestBase
{
    public string SiteTypes { get; set; }
    public string States { get; set; }
    public string WaterSourceTypes { get; set; }
    public string SiteUuids { get; set; }
    public string OverlayUuids { get; set; }
    public string AllocationUuids { get; set; }
}