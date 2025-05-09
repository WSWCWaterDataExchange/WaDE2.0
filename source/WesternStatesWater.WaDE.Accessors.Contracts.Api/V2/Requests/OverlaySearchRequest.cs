using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public class OverlaySearchRequest : SearchRequestBase
{
    public List<string> OverlayUuids { get; set; }
    public List<string> SiteUuids { get; set; }
    public SpatialSearchCriteria GeometrySearch { get; set; }
    public string LastKey { get; set; }
    public List<string> States { get; set; }
    public List<string> OverlayTypes { get; set; }
    public List<string> WaterSourceTypes { get; set; }
}