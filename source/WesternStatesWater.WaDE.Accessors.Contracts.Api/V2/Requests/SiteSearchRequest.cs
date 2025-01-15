using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public class SiteSearchRequest : SearchRequestBase
{
    public SpatialSearchCriteria GeometrySearch { get; set; }
    public string LastSiteUuid { get; set; }
    public List<string> SiteTypes { get; set; }
    public List<string> States { get; set; }
    public List<string> WaterSourcesTypes { get; set; }
}