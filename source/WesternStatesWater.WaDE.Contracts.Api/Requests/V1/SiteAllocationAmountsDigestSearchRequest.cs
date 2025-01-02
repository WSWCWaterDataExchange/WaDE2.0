namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

public class SiteAllocationAmountsDigestSearchRequest : WaterResourceLoadRequestBase
{
    public required SiteAllocationAmountsDigestFilters Filters { get; init; }

    public int StartIndex { get; init; }

    public int RecordCount { get; init; }
}