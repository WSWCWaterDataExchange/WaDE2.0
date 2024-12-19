using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;

public class SiteSearchRequest : SearchRequestBase
{
    public Polygon FilterBoundary { get; set; }
    public int Limit { get; set; }
    public string LastSiteUuid { get; set; }
}