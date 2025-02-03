using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public class TimeSeriesSearchRequest : SearchRequestBase
{
    public long? SiteVariableAmountId { get; set; }
    public SpatialSearchCriteria GeometrySearch { get; set; }
    public List<string> SiteUuids { get; set; }
    public DateRangeFilter DateRange { get; set; }
    public List<string> States { get; set; }
    public List<string> VariableTypes { get; set; }
    public List<string> PrimaryUses { get; set; }
    public List<string> WaterSourceTypes { get; set; }
    public string LastKey { get; set; }
}