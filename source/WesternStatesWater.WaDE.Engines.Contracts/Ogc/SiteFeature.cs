using System.Text.Json.Serialization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc;

public class SiteFeature : IFeature
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    public IAttributesTable Attributes { get; set; }
    public Geometry Geometry { get; set; }

    public Envelope BoundingBox { get; set; }
}