using NetTopologySuite.Features;

namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

// Note: This is not using FeatureCollection because it is a sealed class in NetTopologySuite.
// OGC API requires the class to have Links.
public abstract class FeaturesResponseBase : FormattingResponseBase
{
    public string Type => "FeatureCollection";
    public Feature[] Features { get; set; }
    public Link[] Links { get; set; }
}