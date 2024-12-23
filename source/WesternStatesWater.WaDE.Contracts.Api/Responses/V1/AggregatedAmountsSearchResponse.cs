using WesternStatesWater.Shared.DataContracts;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

public class AggregatedAmountsSearchResponse : ResponseBase
{
    public AggregatedAmounts AggregatedAmounts { get; init; }
}