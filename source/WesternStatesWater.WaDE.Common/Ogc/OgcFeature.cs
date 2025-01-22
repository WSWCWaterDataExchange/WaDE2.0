using System.Text.Json.Serialization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The OGC API specification requires that features include links (e.g., for navigation or related resources).
/// However, the current GeoJSON converters provided by the NetTopologySuite library do not support
/// adding extra custom properties (referred to as "foreign members") to the JSON output.
/// </summary>
public sealed class OgcFeature
{
    [JsonPropertyName("type")] public string Type { get; set; } = "Feature";
    [JsonPropertyName("id")] public required string Id { get; set; }
    [JsonPropertyName("geometry")] public Geometry? Geometry { get; set; }
    [JsonPropertyName("properties")] public AttributesTable? Attributes { get; set; }
    [JsonPropertyName("links")] public Link[]? Links { get; set; }
}