﻿using System;
using System.Collections.Generic;
using Bogus;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
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

    public static class RandomizerExtensions
    {
        public static string Word(this Randomizer r, int maxLength)
        {
            string result;
            do
            {
                result = r.Word();
            } while (result.Length > maxLength);

            return result;
        }
    }

    public class Geography
    {
        public Geography(Faker f)
        {
            _faker = f;
        }

        private readonly Faker _faker;

        private static readonly string[] geoWktStrings = new[]
        {
            "POINT(-96.7014 40.8146)",
            "POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))",
            "MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)),((20 35, 45 20, 30 5, 10 10, 10 30, 20 35),(30 20, 20 25, 20 15, 30 20)))"
        };

        private static readonly string[] geoJsonStrings = new[]
        {
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

        public Geometry Geometry()
        {
            return GeometryExtensions.GetGeometryByWkt(GeometryWktString());
        }

        public Geometry RandomPoint(
            double minLatitude = -90,
            double maxLatitude = 90,
            double minLongitude = -180,
            double maxLongitude = 180
        )
        {
            var longitude = _faker.Random.Double(minLongitude, maxLongitude);
            var latitude = _faker.Random.Double(minLatitude, maxLatitude);
            var wtk = $"POINT({longitude} {latitude})";
            var geometry = GeometryExtensions.GetGeometryByWkt(wtk);
            
            return geometry;
        }

        public Geometry RandomMultiPoint(
            double minLatitude = -90,
            double maxLatitude = 90,
            double minLongitude = -180,
            double maxLongitude = 180,
            int numberOfPoints = 5
        )
        {
            var points = new List<string>();

            for (var i = 0; i < numberOfPoints; i++)
            {
                var longitude = _faker.Random.Double(minLongitude, maxLongitude);
                var latitude = _faker.Random.Double(minLatitude, maxLatitude);

                points.Add($"{longitude} {latitude}");
            }

            var wtk = $"MULTIPOINT ({string.Join(", ", points)})";
            var geometry = GeometryExtensions.GetGeometryByWkt(wtk);
            
            return geometry;
        }

        public Geometry RandomPolygon(
            double minLatitude = -90,
            double maxLatitude = 90,
            double minLongitude = -180,
            double maxLongitude = 180,
            int numberOfVertices = 5
        )
        {
            var coordinates = new List<string>();

            // Generate random latitude and longitude points
            for (var i = 0; i < numberOfVertices; i++)
            {
                var latitude = _faker.Random.Double(minLatitude, maxLatitude);
                var longitude = _faker.Random.Double(minLongitude, maxLongitude);
                coordinates.Add($"{longitude} {latitude}");
            }

            // Close the polygon by repeating the first point
            coordinates.Add(coordinates[0]);

            var wtk = $"POLYGON (({string.Join(", ", coordinates)}))";
            var geometry = GeometryExtensions.GetGeometryByWkt(wtk);
            
            return geometry!.IsValid ? geometry : GeometryFixer.Fix(geometry);
        }

        public Geometry RandomMultiPolygon(
            double minLatitude = -90,
            double maxLatitude = 90,
            double minLongitude = -180,
            double maxLongitude = 180,
            int numberOfPolygons = 2,
            int numberOfVertices = 5
        )
        {
            var polygons = new List<string>();

            for (var i = 0; i < numberOfPolygons; i++)
            {
                var coordinates = new List<string>();

                // Generate random latitude and longitude points
                for (var j = 0; j < numberOfVertices; j++)
                {
                    var latitude = _faker.Random.Double(minLatitude, maxLatitude);
                    var longitude = _faker.Random.Double(minLongitude, maxLongitude);
                    coordinates.Add($"{longitude} {latitude}");
                }

                // Close the polygon by repeating the first point
                coordinates.Add(coordinates[0]);

                polygons.Add($"(({string.Join(", ", coordinates)}))");
            }

            var wtk = $"MULTIPOLYGON ({string.Join(", ", polygons)})";
            var geometry = GeometryExtensions.GetGeometryByWkt(wtk);

            return geometry!.IsValid ? geometry : GeometryFixer.Fix(geometry, isKeepMulti: true);
        }
    }
}