using NetTopologySuite.Features;

namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

public class SiteFeaturesResponse : FeaturesResponseBase
{
    public string Type { get; set; } = "FeatureCollection";
    
    // Note: This is not using FeatureCollection because it is a sealed class.
    // OGC API requires the parent class to have Links
    public SiteFeature[] SiteFeatures { get; set; } = [];
    public Feature[] Features { get; set; } = [];
    public Link[] Links { get; set; } = [];
}