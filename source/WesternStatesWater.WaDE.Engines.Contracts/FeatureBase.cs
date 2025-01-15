using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public abstract class FeatureBase
{
    public Geometry Geometry { get; set; }
    [JsonPropertyName("id")]
    public string Id { get; set; }
}