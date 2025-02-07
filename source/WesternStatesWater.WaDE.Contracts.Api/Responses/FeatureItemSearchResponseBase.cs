using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Common.Ogc;

namespace WesternStatesWater.WaDE.Contracts.Api.Responses;

public abstract class FeatureItemSearchResponseBase : WaterResourceSearchResponseBase, IOgcFeature
{
    public string Type => "Feature";
    public string Id { get; set; }
    public Geometry Geometry { get; set; }
    public AttributesTable Properties { get; set; }
    public Link[] Links { get; set; }
}