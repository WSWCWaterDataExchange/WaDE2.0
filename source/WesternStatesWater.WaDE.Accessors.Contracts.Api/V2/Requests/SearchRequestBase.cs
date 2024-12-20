using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;

public abstract class SearchRequestBase : RequestBase
{
    public int Limit { get; set; }
}