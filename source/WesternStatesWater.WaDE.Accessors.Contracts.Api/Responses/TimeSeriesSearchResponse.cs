using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

public class TimeSeriesSearchResponse : SearchResponseBase
{
    public List<SiteVariableAmount> TimeSeries { get; set; }
}