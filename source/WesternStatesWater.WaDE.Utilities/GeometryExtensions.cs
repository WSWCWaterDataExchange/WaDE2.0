using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace WesternStatesWater.WaDE.Utilities
{
    public static class GeometryExtensions
    {
        public static string? AsGeoJson(this Geometry geometry)
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
                return stringWriter.ToString();
            }
        }
    }
}