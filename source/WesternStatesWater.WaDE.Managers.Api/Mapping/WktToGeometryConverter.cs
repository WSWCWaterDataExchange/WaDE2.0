using AutoMapper;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

/// <summary>
/// Value converter to convert a WKT string to WTS84 Geometry.
/// </summary>
public class WktToGeometryConverter : IValueConverter<string, Geometry>
{
    private const int SRID_WGS84 = 4326;
    private static readonly WKTReader WktReader = new(new NtsGeometryServices(PrecisionModel.Floating.Value, SRID_WGS84));
    Geometry IValueConverter<string, Geometry>.Convert(string sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
        {
            return null;
        }
        
        return WktReader.Read(sourceMember);
    }
}