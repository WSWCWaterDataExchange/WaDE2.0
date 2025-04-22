namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

public class OverlayResourceSearchRequest : WaterResourceLoadRequestBase
{
    public OverlayFilters Filters { get; init; }

    public int StartIndex { get; init; }

    public int RecordCount { get; init; }

    public GeometryFormat OutputGeometryFormat { get; init; }
}