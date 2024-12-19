using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

public class AllocationSearchResponse : SearchResponseBase
{
    public List<Allocation> Allocations { get; set; }
}