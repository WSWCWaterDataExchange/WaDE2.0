using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Common.Ogc;

public interface IOgcFeature
{
    public string? Type { get; }
    public string? Id { get; set; }
    public Geometry? Geometry { get; set; }
    public AttributesTable? Properties { get; set; }
    public Link[]? Links { get; set; }
}