using WesternStatesWater.Shared.DataContracts;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;

public abstract class SearchResponseBase : ResponseBase
{
    public string LastUuid { get; set; }
}