using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public class SpatialSearchCriteria
{
    public Geometry Geometry { get; set; }
    public SpatialRelationType SpatialRelationType { get; set; }
}