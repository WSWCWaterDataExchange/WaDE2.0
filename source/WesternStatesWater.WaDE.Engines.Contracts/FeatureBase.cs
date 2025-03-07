using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Engines.Contracts;

public abstract class FeatureBase
{
    public Geometry Geometry { get; set; }
    public string Id { get; set; }
}