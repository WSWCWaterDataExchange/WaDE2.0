using System.Text.Json.Serialization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Contracts.Api.OgcApi;

// TODO: Put this in common or somewhere for reusability
// Might need to handle empty constructor for deserialization
public class SiteFeature : IFeature
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    public IAttributesTable Attributes { get; set; }
    public Geometry Geometry { get; set; }

    public Envelope BoundingBox { get; set; }
}