using NetTopologySuite.Features;
using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses;

public abstract class FeaturesSearchResponseBase : ResponseBase
{
    public string Type => "FeatureCollection";
    public Feature[] Features { get; set; }
    public Link[] Links { get; set; }
}