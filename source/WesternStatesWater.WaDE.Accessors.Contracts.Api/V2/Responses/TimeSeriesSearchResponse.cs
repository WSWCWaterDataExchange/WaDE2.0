using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;

public class TimeSeriesSearchResponse : SearchResponseBase
{
    public List<TimeSeriesSearchItem> Sites { get; set; }
}