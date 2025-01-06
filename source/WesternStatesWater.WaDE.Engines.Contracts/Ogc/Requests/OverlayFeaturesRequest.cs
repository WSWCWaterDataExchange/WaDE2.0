namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;

public class OverlayFeaturesRequest : FeaturesRequestBase
{
    public List<string>? OverlayUuids { get; set; }
    public List<string>? SiteUuids { get; set; }
    public double[][]? Bbox { get; set; }
    public string? LastOverlayUuid { get; set; }
}