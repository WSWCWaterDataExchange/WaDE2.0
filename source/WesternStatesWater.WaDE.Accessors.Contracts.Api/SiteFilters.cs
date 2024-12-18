using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public class SiteFilters
{
    public Polygon FilterBoundary { get; set; }
    public int Limit { get; set; }
    public string LastSiteUuid { get; set; }
}