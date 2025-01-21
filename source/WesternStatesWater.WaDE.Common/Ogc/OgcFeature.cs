using System.Text.Json.Serialization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// Represents a GeoJSON Feature object around the OGC API Specification.
/// </summary>
/// <remarks>Due to OGC API specification of having links on the Feature. The current NetTopologySuite GeoJson converters
/// do not support foreign members.</remarks>
public sealed class OgcFeature
{
    [JsonPropertyName("type")] public string Type { get; set; } = "Feature";
    [JsonPropertyName("id")] public required string Id { get; set; }
    [JsonPropertyName("geometry")] public Geometry? Geometry { get; set; }
    [JsonPropertyName("properties")] public AttributesTable? Attributes { get; set; }
    [JsonPropertyName("links")] public Link[]? Links { get; set; }
}