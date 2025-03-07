namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

public class SiteAllocationAmountsSearchResponse : WaterResourceLoadResponseBase
{
    public required WaterAllocations WaterAllocations { get; init; }
}