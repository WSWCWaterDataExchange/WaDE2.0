using Bogus;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Tests.Helpers
{
    public static class FakerExtensions
    {
        public static Geography Geography(this Faker f)
        {
            return new Geography(f);
        }
    }

    public class Geography
    {
        public Geography(Faker f)
        {
            _faker = f;
        }

        private readonly Faker _faker;

        private static readonly string[] geoWktStrings = new[] {
            "POINT(-96.7014 40.8146)",
            "POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))",
            "MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)),((20 35, 45 20, 30 5, 10 10, 10 30, 20 35),(30 20, 20 25, 20 15, 30 20)))"
        };

        private static readonly string[] geoJsonStrings = new[] {
            "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}",
            "{\"type\":\"Polygon\",\"coordinates\":[[[-96.7015,40.8149],[-96.7012,40.8149],[-96.7012,40.8146],[-96.7015,40.8146],[-96.7015,40.8149]]]}",
            "{\"type\":\"MultiPolygon\",\"coordinates\":[[[[40.0,40.0],[20.0,45.0],[45.0,30.0],[40.0,40.0]]],[[[20.0,35.0],[45.0,20.0],[30.0,5.0],[10.0,10.0],[10.0,30.0],[20.0,35.0]],[[30.0,20.0],[20.0,25.0],[20.0,15.0],[30.0,20.0]]]]}",
        };

        public string GeometryString()
        {
            return _faker.PickRandom(GeometryGeoJsonString(), GeometryWktString());
        }

        public string GeometryGeoJsonString()
        {
            return _faker.PickRandom(geoJsonStrings);
        }

        public string GeometryWktString()
        {
            return _faker.PickRandom(geoWktStrings);
        }

        public NetTopologySuite.Geometries.Geometry Geometry()
        {
            return GeometryExtensions.GetGeometryByWkt(GeometryWktString());
        }
    }
}