namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesItemRequest : SiteFeaturesSearchRequestBase
{
    public string SiteTypes { get; set; }
    public string States { get; set; }
}