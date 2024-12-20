using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;

public class AllocationSearchResponse : SearchResponseBase
{
    public List<AllocationSearchItem> Allocations { get; set; }
}