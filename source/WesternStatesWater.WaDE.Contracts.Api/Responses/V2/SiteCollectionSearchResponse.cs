using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

public class SiteCollectionSearchResponse : ResponseBase
{
    public required Link[] Links { get; set; } = [];
    public required Collection[] Collections { get; set; } = [];
    public string Title { get; set; }
    public string Description { get; set; }
}