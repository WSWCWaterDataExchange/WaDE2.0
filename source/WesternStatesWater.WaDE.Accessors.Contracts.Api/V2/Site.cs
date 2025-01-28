using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;

public class Site
{
    public string SiteUuid { get; set; }
    public string SiteNativeId { get; set; }
    public string SiteName { get; set; }
    public string UsgsSiteId { get; set; }
    public string SiteType { get; set; }
    public Geometry Location { get; set; }
    public string CoordinateMethod { get; set; }
    public string CoordinateAccuracy { get; set; }
    public string GnisCode { get; set; }
    public string EpsgCode { get; set; }
    public string NhdNetworkStatus { get; set; }
    public string NhdProduct { get; set; }
    public string State { get; set; }
    public string Huc8 { get; set; }
    public string Huc12 { get; set; }
    public string County { get; set; }
    public string PodOrPouSite { get; set; }
    public WaterSourceSummary[] WaterSources { get; set; }
}