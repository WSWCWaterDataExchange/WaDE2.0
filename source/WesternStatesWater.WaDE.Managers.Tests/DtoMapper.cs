using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Mapping;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Managers.Tests
{
    [TestClass]
    public class DtoMapper
    {
        [TestMethod]
        public void ConfigurationIsValid()
        {
            Mapping.DtoMapper.Configuration.AssertConfigurationIsValid();
        }

        [DataTestMethod]
        [DataRow(null, false, null, null)]
        [DataRow(null, true, null, null)]
        [DataRow(null, true, "true", null)]
        [DataRow(null, true, GeometryFormat.Wkt, null)]
        [DataRow(null, true, GeometryFormat.GeoJson, null)]
        [DataRow("POINT(-96.7014 40.8146)", false, null, GeometryFormat.Wkt)]
        [DataRow("POINT(-96.7014 40.8146)", true, null, GeometryFormat.Wkt)]
        [DataRow("POINT(-96.7014 40.8146)", true, "true", GeometryFormat.Wkt)]
        [DataRow("POINT(-96.7014 40.8146)", true, GeometryFormat.Wkt, GeometryFormat.Wkt)]
        [DataRow("POINT(-96.7014 40.8146)", true, GeometryFormat.GeoJson, GeometryFormat.GeoJson)]
        public void Map_GeometryToString(string geometryString, bool hasKey, object key, GeometryFormat? expectedGeometryFormat)
        {
            Geometry geometry = null;
            string expectedResult = null;
            if (geometryString != null)
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var reader = new WKTReader(geometryFactory.GeometryServices);
                geometry = reader.Read(geometryString);
                expectedResult = expectedGeometryFormat.Value == GeometryFormat.GeoJson ? geometry.AsGeoJson() : geometry.AsText();
            }

            Action<IMappingOperationOptions> mappingOperationsAction = hasKey ? a => a.Items.Add(ApiProfile.GeometryFormatKey, key) : null;

            var result = geometry.Map<string>(mappingOperationsAction);
            result.Should().Be(expectedResult);
        }
    }
}
