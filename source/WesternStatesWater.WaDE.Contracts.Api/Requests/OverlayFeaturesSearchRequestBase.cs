namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public abstract class OverlayFeaturesSearchRequestBase : FeaturesSearchRequestBase
{
    public string OverlayUuids { get; set; }
    public string SiteUuids { get; set; }
    public string States { get; set; }
    public string OverlayTypes { get; set; }
    public string WaterSourceTypes { get; set; }
}