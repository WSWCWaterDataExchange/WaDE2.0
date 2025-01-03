using WesternStatesWater.Shared.DataContracts;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public abstract class FeaturesSearchRequestBase : RequestBase
{
    public string Bbox { get; set; }
    public string Limit { get; set; }
    public string Next { get; set; }
}