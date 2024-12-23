namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;

public class SiteFeaturesRequest : FeaturesRequestBase
{
    public double[][]? BoundingBox { get; set; }
    public string? LastSiteUuid { get; set; }
}