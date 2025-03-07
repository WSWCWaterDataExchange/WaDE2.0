using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Common.Ogc;

/// <summary>
/// The OGC API specification requires that features include links (e.g., for navigation or related resources).
/// However, the current GeoJSON converters provided by the NetTopologySuite library do not support
/// adding extra custom properties (referred to as "foreign members") to the JSON output.
/// </summary>
public sealed class OgcFeature : IOgcFeature
{
    public string Type => "Feature";
    public string Id { get; set; }
    public Geometry? Geometry { get; set; }
    public AttributesTable? Properties { get; set; }
    public Link[]? Links { get; set; }
}