using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;


namespace WesternStatesWater.WaDE.Utilities
{
    public static class GeometryExtensions
    {
        public static dynamic? AsGeoJson(this Geometry geometry)
        {
            if (geometry == null)
            {
                return null;
            }
            var serializer = GeoJsonSerializer.Create();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                serializer.Serialize(jsonWriter, geometry);
                return (dynamic)JsonConvert.DeserializeObject(stringWriter.ToString());
            }
        }

        //Json can start with lots of characters but I _believe_ GeoJson can only start with {
        private static readonly string[] GeoJsonStartingCharacters = new[] { "{" };

        public static Geometry? GetGeometry(string geometry)
        {
            if (string.IsNullOrWhiteSpace(geometry))
            {
                return null;
            }
            if (IsGeoJsonString(geometry))
            {
                //We are assuming it is GeoJson but it might not be well formatted
                return GetGeometryByGeoJson(geometry);
            }
            return GetGeometryByWkt(geometry);

        }

        public static Geometry? GetGeometryByWkt(string wkt)
        {
            if (string.IsNullOrWhiteSpace(wkt))
            {
                return null;
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326)!;
            var reader = new WKTReader(geometryFactory.GeometryServices);
            return reader.Read(wkt);
        }

        public static Geometry? GetGeometryByGeoJson(string geoJson)
        {
            if (string.IsNullOrWhiteSpace(geoJson))
            {
                return null;
            }
            var serializer = GeoJsonSerializer.Create();
            using (var stringReader = new StringReader(geoJson))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                return serializer.Deserialize<Geometry>(jsonReader);
            }
        }

        private static bool IsGeoJsonString(string geometry)
        {
            return GeoJsonStartingCharacters.Any(geometry.TrimStart().StartsWith);
        }
    }
}