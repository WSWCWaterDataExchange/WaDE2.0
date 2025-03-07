namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

public class AggregatedAmountsSearchResponse : WaterResourceLoadResponseBase
{
    public AggregatedAmounts AggregatedAmounts { get; init; }
}