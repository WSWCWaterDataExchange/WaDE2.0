using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Responses;

public class SiteExtentSearchResponse : SiteSearchResponseBase
{
    public BoundaryBox BoundaryBox { get; set; }
    public DateTime? TimeframeStart { get; set; }
    public DateTime? TimeframeEnd { get; set; }
}