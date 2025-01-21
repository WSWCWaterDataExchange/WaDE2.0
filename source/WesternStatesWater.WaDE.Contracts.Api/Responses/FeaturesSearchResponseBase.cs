using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.WaDE.Common.Ogc;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses;

public abstract class FeaturesSearchResponseBase : ResponseBase
{
    public string Type => "FeatureCollection";
    public OgcFeature[] Features { get; set; }
    public Link[] Links { get; set; }
}