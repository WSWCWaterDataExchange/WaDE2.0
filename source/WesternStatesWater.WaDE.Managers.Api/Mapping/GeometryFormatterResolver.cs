using AutoMapper;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

public class GeometryFormatterResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, object>
{
    private readonly string _format;
    private readonly string _propertyName;

    public GeometryFormatterResolver(string format, string propertyName)
    {
        _format = format;
        _propertyName = propertyName;
    }
    public object Resolve(TSource source, TDestination destination, object destMember, ResolutionContext context)
    {
        var geometryProperty = typeof(TSource).GetProperty(_propertyName);
        if (geometryProperty == null)
        {
            return null;
        }
        
        var geometry = (Geometry)geometryProperty.GetValue(source);
        if (geometry == null)
        {
            return null;
        }
        
        var selectedFormat = ManagerApi.GeometryFormat.Wkt;
        if (context.Items.TryGetValue(_format, out var obj))
        {
            selectedFormat = obj as ManagerApi.GeometryFormat? ?? ManagerApi.GeometryFormat.Wkt;
        }

        if (selectedFormat == ManagerApi.GeometryFormat.GeoJson)
        {
            return geometry.AsGeoJson();
        }
        return geometry.AsText();
    }
}