using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class OverlayFeature : FeatureBase
{
    [JsonPropertyName("nId")]
    public string OverlayNativeId { get; set; }
    [JsonPropertyName("name")]
    public string RegulatoryName { get; set; }
    [JsonPropertyName("desc")]
    public string RegulatoryDescription { get; set; }
    [JsonPropertyName("status")]
    public string RegulatoryStatus { get; set; }
    [JsonPropertyName("agency")]
    public string OversightAgency { get; set; }
    [JsonPropertyName("statute")]
    public string RegulatoryStatute { get; set; }
    [JsonPropertyName("statuteRef")]
    public string RegulatoryStatuteLink { get; set; }
    [JsonPropertyName("statueEffDate")]
    public string StatutoryEffectiveDate { get; set; }
    [JsonPropertyName("statueEndDate")]
    public string StatutoryEndDate { get; set; }
    [JsonPropertyName("oType")]
    public string OverlayType { get; set; }
    [JsonPropertyName("wSource")]
    public string WaterSource { get; set; }
    [JsonPropertyName("areaNames")]
    public string[] AreaNames { get; set; }
    [JsonPropertyName("areaNIds")]
    public string[] AreaNativeIds { get; set; }
    [JsonPropertyName("sites")]
    public string[] SiteUuids { get; set; }
    [JsonPropertyName("states")]
    public string[] States { get; set; }
}