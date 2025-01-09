namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeaturesSearchRequest : FeaturesSearchRequestBase
{    
    public string OverlayUuids { get; set; }
    public string SiteUuids { get; set; }
}