using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public abstract class MetadataBase
{
    public BoundaryBox BoundaryBox { get; set; }
    public DateTime? IntervalStartDate { get; set; }
    public DateTime? IntervalEndDate { get; set; }
}