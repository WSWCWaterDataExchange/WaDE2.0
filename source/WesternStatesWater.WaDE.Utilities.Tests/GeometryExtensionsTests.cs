using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace WesternStatesWater.WaDE.Utilities.Tests;

[TestClass]
public class GeometryExtensionsTests
{
    [DataTestMethod]
    [DataRow(null, null)]
    [DataRow("POINT(-96.7014 40.8146)", "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}")]
    [DataRow("POLYGON((-96.7015 40.8149,-96.7012 40.8149,-96.7012 40.8146,-96.7015 40.8146,-96.7015 40.8149))", "{\"type\":\"Polygon\",\"coordinates\":[[[-96.7015,40.8149],[-96.7012,40.8149],[-96.7012,40.8146],[-96.7015,40.8146],[-96.7015,40.8149]]]}")]
    [DataRow("MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)),((20 35, 45 20, 30 5, 10 10, 10 30, 20 35),(30 20, 20 25, 20 15, 30 20)))", "{\"type\":\"MultiPolygon\",\"coordinates\":[[[[40.0,40.0],[20.0,45.0],[45.0,30.0],[40.0,40.0]]],[[[20.0,35.0],[45.0,20.0],[30.0,5.0],[10.0,10.0],[10.0,30.0],[20.0,35.0]],[[30.0,20.0],[20.0,25.0],[20.0,15.0],[30.0,20.0]]]]}")]
    public void TestMethod1(string wkt, string expectedGeoJson)
    {
        Geometry? geometry = null;
        if (wkt != null)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var reader = new WKTReader(geometryFactory.GeometryServices);
            geometry = reader.Read(wkt);
        }

        var result = geometry.AsGeoJson();
        result.Should().Be(expectedGeoJson);
    }
}