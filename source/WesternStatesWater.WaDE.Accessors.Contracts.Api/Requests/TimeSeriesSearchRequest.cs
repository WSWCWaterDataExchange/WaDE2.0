using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;

public class TimeSeriesSearchRequest : SearchRequestBase
{
    public List<string> SiteUuids { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long? LastKey { get; set; }
    public int Limit { get; set; }
}