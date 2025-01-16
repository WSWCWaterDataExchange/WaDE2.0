using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class WaterSourceSummary
{
    [JsonPropertyName("id")]
    public required string WaterSourceUuId { get; set; }
    [JsonPropertyName("nId")]
    public string? NativeId { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("sourceType")]
    public required string SourceType { get; set; }
    [JsonPropertyName("type")]
    public required string WaterType { get; set; }
}