using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Engines.Contracts.Attributes;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public abstract class FeatureBase
{
    public Geometry Geometry { get; set; }
    [FeaturePropertyName("id")]
    public string Id { get; set; }
}