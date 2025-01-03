using WesternStatesWater.Shared.DataContracts;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public abstract class FeaturesSearchRequestBase : RequestBase
{
    public string Bbox { get; set; }
    public string Limit { get; set; }
    public string Next { get; set; }
}