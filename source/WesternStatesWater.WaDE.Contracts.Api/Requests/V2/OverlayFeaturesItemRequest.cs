namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeaturesItemRequest : OverlayFeaturesSearchRequestBase
{    
    public string OverlayUuids { get; set; }
    public string SiteUuids { get; set; }
    public string States { get; set; }
    public string OverlayTypes { get; set; }
    public string WaterSourceTypes { get; set; }
}