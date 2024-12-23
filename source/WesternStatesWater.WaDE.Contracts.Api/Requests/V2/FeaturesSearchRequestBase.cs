using WesternStatesWater.Shared.DataContracts;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public abstract class FeaturesSearchRequestBase : RequestBase
{
    public int? Limit { get; set; }
}