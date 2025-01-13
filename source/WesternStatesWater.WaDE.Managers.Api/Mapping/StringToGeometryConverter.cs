using AutoMapper;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

public class StringToGeometryConverter : IValueConverter<string, Geometry>
{
    // Spatial Reference ID for WGS84
    private const int SRID_WGS84 = 4326;
    public Geometry Convert(string sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
        {
            return null;
        }

        var reader = new NetTopologySuite.IO.WKTReader(new NetTopologySuite.NtsGeometryServices(PrecisionModel.Floating.Value, SRID_WGS84));
        return reader.Read(sourceMember);
    }
}