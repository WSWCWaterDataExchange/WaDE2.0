namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesSearchRequest : FeaturesSearchRequestBase
{
    public string LastSiteUuid { get; set; }
    public double[][] Bbox { get; set; }
}