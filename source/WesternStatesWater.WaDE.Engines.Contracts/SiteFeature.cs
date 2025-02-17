namespace WesternStatesWater.WaDE.Engines.Contracts;

public class SiteFeature : FeatureBase
{
    public string SiteUuid { get; set; }
    public string? SiteNativeId { get; set; }
    public string? SiteName { get; set; }
    public string? UsgsSiteId { get; set; }
    public string? SiteTypeCv { get; set; }
    public string? SiteTypeWaDEName { get; set; }
    public string? CoordinateMethodCv { get; set; }
    public string? CoordinateAccuracy { get; set; }
    public string? GnisCodeCv { get; set; }
    public string? EpsgCodeCv { get; set; }
    public string? NhdNetworkStatusCv { get; set; }
    public string? NhdProductCv { get; set; }
    public string? StateCv { get; set; }
    public string? Huc8 { get; set; }
    public string? Huc12 { get; set; }
    public string? County { get; set; }
    public string? PodOrPouSite { get; set; }
    public WaterSourceSummary[]? WaterSources { get; set; }
}