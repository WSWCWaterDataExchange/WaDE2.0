namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

public class SiteAllocationAmountsSearchRequest : WaterResourceLoadRequestBase
{
    public required SiteAllocationAmountsFilters Filters { get; init; }

    public int StartIndex { get; init; }

    public int RecordCount { get; init; }

    public GeometryFormat OutputGeometryFormat { get; init; }
}