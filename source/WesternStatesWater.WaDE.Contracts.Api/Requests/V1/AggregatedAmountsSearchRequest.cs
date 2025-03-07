namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

public class AggregatedAmountsSearchRequest : WaterResourceLoadRequestBase
{
    public required AggregatedAmountsFilters Filters { get; init; }

    public int StartIndex { get; init; }

    public int RecordCount { get; init; }

    public GeometryFormat OutputGeometryFormat { get; init; }
}