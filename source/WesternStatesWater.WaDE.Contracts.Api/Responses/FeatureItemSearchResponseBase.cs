using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.WaDE.Common.Ogc;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses;

public abstract class FeatureItemSearchResponseBase : WaterResourceSearchResponseBase
{
    public OgcFeature Feature { get; set; }
}