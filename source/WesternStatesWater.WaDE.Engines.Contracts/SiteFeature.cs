using System.Text.Json.Serialization;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class SiteFeature : FeatureBase
{
    [JsonPropertyName("siteUuid")]
    public string SiteUuid { get; set; }
    [JsonPropertyName("nId")]
    public string? SiteNativeId { get; set; }
    [JsonPropertyName("name")]
    public string? SiteName { get; set; }
    [JsonPropertyName("usgsSiteId")]
    public string? UsgsSiteId { get; set; }
    [JsonPropertyName("sType")]
    public string? SiteType { get; set; }
    [JsonPropertyName("coordMethod")]
    public string? CoordinateMethod { get; set; }
    [JsonPropertyName("coordAcc")]
    public string? CoordinateAccuracy { get; set; }
    [JsonPropertyName("gnisCode")]
    public string? GnisCode { get; set; }
    [JsonPropertyName("epsgCode")]
    public string? EpsgCode { get; set; }
    [JsonPropertyName("nhdNetStat")]
    public string? NhdNetworkStatus { get; set; }
    [JsonPropertyName("nhdProd")]
    public string? NhdProduct { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
    [JsonPropertyName("huc8")]
    public string? Huc8 { get; set; }
    [JsonPropertyName("huc12")]
    public string? Huc12 { get; set; }
    [JsonPropertyName("county")]
    public string? County { get; set; }
    [JsonPropertyName("podOrPouSite")]
    public string? PodOrPouSite { get; set; }
    [JsonPropertyName("waterSources")]
    public WaterSourceSummary[]? WaterSources { get; set; }
}