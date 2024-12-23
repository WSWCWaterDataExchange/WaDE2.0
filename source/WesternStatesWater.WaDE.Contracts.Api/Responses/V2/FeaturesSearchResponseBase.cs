using NetTopologySuite.Features;
using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

public abstract class FeaturesSearchResponseBase : ResponseBase
{
    public string Type { get; set; }
    public SiteFeature[] SiteFeatures { get; set; }
    public Feature[] Features { get; set; }
    public Link[] Links { get; set; }
}