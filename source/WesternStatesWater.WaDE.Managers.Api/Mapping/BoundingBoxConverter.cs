using System.Linq;
using AutoMapper;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

/// <summary>
/// Value converter to convert a bounding box string "-180,-90,180,90" to a Geometry type.
/// </summary>
public class BoundingBoxConverter : IValueConverter<string, Geometry>
{
    public Geometry Convert(string sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
        {
            return null;
        }

        GeometryFactory geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

        var bbox = sourceMember.Split(",").Select(double.Parse).ToArray();

        double left = bbox[0];
        double bottom = bbox[1];
        double right = bbox[2];
        double top = bbox[3];

        if (left > right) // Crosses the anti meridian
        {
            var box1 = geometryFactory.CreatePolygon([
                new Coordinate(left, top),
                new Coordinate(180, top),
                new Coordinate(180, bottom),
                new Coordinate(left, bottom),
                new Coordinate(left, top)
            ]);

            var box2 = geometryFactory.CreatePolygon([
                new Coordinate(-180, top),
                new Coordinate(right, top),
                new Coordinate(right, bottom),
                new Coordinate(-180, bottom),
                new Coordinate(-180, top)
            ]);

            return geometryFactory.BuildGeometry([box1, box2]);
        }

        var box = geometryFactory.CreatePolygon([
            new Coordinate(left, top),
            new Coordinate(right, top),
            new Coordinate(right, bottom),
            new Coordinate(left, bottom),
            new Coordinate(left, top)
        ]);

        return geometryFactory.BuildGeometry([box]);
    }
}