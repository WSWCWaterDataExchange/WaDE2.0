namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public class SiteSearchRequest : SearchRequestBase
{
    public SpatialSearchCriteria GeometrySearch { get; set; }
    public string LastSiteUuid { get; set; }
    public string[] SiteTypes { get; set; }
}