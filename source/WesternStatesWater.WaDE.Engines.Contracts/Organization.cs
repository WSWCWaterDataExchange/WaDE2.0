using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class Organization
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }
    [JsonPropertyName("purview")]
    public string Purview { get; set; }
    [JsonPropertyName("website")]
    public string Website { get; set; }
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }
    [JsonPropertyName("contactName")]
    public string ContactName { get; set; }
    [JsonPropertyName("contactEmail")]
    public string ContactEmail { get; set; }
    [JsonPropertyName("state")]
    public string State { get; set; }
}