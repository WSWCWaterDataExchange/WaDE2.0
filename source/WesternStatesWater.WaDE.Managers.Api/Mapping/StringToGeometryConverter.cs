using AutoMapper;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

public class StringToGeometryConverter : IValueConverter<string, Geometry>
{
    public Geometry Convert(string sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
        {
            return null;
        }

        var reader = new NetTopologySuite.IO.WKTReader(new NetTopologySuite.NtsGeometryServices(PrecisionModel.Floating.Value, 4326));
        return reader.Read(sourceMember);
    }
}