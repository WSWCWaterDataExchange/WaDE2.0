using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class Method
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("nemiLink")]
    public string NemiLink { get; set; }
    [JsonPropertyName("applicableResourceType")]
    public string ApplicableResourceType { get; set; }
}