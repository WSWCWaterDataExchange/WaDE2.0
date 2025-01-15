using WesternStatesWater.WaDE.Engines.Contracts.Attributes;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public class SiteFeature : FeatureBase
{
    [FeaturePropertyName("nId")]
    public string? SiteNativeId { get; set; }
    [FeaturePropertyName("name")]
    public string? SiteName { get; set; }
    [FeaturePropertyName("usgsSiteId")]
    public string? UsgsSiteId { get; set; }
    [FeaturePropertyName("sType")]
    public string? SiteType { get; set; }
    [FeaturePropertyName("coordMethod")]
    public string? CoordinateMethod { get; set; }
    [FeaturePropertyName("coordAcc")]
    public string? CoordinateAccuracy { get; set; }
    [FeaturePropertyName("gnisCode")]
    public string? GnisCode { get; set; }
    [FeaturePropertyName("epsgCode")]
    public string? EpsgCode { get; set; }
    [FeaturePropertyName("nhdNetStat")]
    public string? NhdNetworkStatus { get; set; }
    [FeaturePropertyName("nhdProd")]
    public string? NhdProduct { get; set; }
    [FeaturePropertyName("state")]
    public string? State { get; set; }
    [FeaturePropertyName("huc8")]
    public string? Huc8 { get; set; }
    [FeaturePropertyName("huc12")]
    public string? Huc12 { get; set; }
    [FeaturePropertyName("county")]
    public string? County { get; set; }
    [FeaturePropertyName("rightUuids")]
    public string[]? RightUuids { get; set; }
    [FeaturePropertyName("isTimeSeries")]
    public bool? IsTimeSeries { get; set; }
    [FeaturePropertyName("podOrPouSite")]
    public string? PodOrPouSite { get; set; }
    [FeaturePropertyName("waterSources")]
    public WaterSourceSummary[]? WaterSources { get; set; }
    [FeaturePropertyName("overlays")]
    public string[]? Overlays { get; set; }
}