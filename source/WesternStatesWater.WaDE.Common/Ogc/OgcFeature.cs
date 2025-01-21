using System.Text.Json.Serialization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// This class inherits IFeature because NetTopologySuite.Features.Feature is sealed and OGC API Specification
/// requires links to be included in the GeoJson feature.
/// Code was copied from the NetTopologySuite.Features.Feature class and modified to include links.
/// </summary>
public sealed class OgcFeature
{
    [JsonPropertyName("type")] public string Type { get; set; } = "Feature";
    [JsonPropertyName("id")] public required string Id { get; set; }
    [JsonPropertyName("geometry")] public Geometry? Geometry { get; set; }
    [JsonPropertyName("properties")] public AttributesTable? Attributes { get; set; }
    [JsonPropertyName("links")] public Link[]? Links { get; set; }
}