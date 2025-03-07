namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

public class SiteVariableAmountsSearchRequest : WaterResourceLoadRequestBase
{
    public required SiteVariableAmountsFilters Filters { get; init; }

    public int StartIndex { get; init; }

    public int RecordCount { get; init; }

    public GeometryFormat OutputGeometryFormat { get; init; }
}