using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public class SiteSearchRequest : SearchRequestBase
{
    public Polygon FilterBoundary { get; set; }
    public string LastSiteUuid { get; set; }
}