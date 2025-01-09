using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
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
        public void Map_GeometryToObject(string geometryString, bool hasKey, object key,
            GeometryFormat? expectedGeometryFormat)
        {
            Geometry geometry = null;
            if (geometryString != null)
            {
                var reader = new WKTReader(new NtsGeometryServices(PrecisionModel.Floating.Value, 4326));
                geometry = reader.Read(geometryString);
            }

            var source = new Accessors.Contracts.Api.WaterSource
            {
                WaterSourceGeometry = geometry,
            };

            Action<IMappingOperationOptions> mappingOperationsAction =
                hasKey ? a => a.Items.Add(ApiProfile.GeometryFormatKey, key) : null;

            var result = source.Map<WaterSource>(mappingOperationsAction);
            if (expectedGeometryFormat == null)
            {
                result.WaterSourceGeometry.Should().BeNull();
            }
            else
            {
                var expectedResult = expectedGeometryFormat == GeometryFormat.Wkt
                    ? geometry.AsText()
                    : geometry.AsGeoJson();
                result.WaterSourceGeometry.ToString().Should().Be(expectedResult.ToString());
            }
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("10,10,20,20", "POLYGON ((10 20, 20 20, 20 10, 10 10, 10 20))")]
        [DataRow("170, 50, -160, 60",
            "MULTIPOLYGON (((170 60, 180 60, 180 50, 170 50, 170 60)), ((-180 60, -160 60, -160 50, -180 50, -180 60)))")] // Crosses antimeridian
        public void Map_StringToBoundaryBox(string input, string expected)
        {
            var request = new SiteFeaturesSearchRequest
            {
                Bbox = input
            };

            var response = request.Map<SiteSearchRequest>();
            if (response.FilterBoundary == null)
                response.FilterBoundary.Should().BeNull();
            else
                response.FilterBoundary.ToString().Should().Be(expected);
        }
    }
}