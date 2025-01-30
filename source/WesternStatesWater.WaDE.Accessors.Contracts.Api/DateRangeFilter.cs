using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public class DateRangeFilter
{
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}