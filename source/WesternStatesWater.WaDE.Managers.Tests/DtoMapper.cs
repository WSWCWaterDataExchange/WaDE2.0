using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
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
        [DataRow(null, true, "string", null)]
        [DataRow(null, true, false, null)]
        [DataRow(null, true, true, null)]
        [DataRow("POINT(-96.7014 40.8146)", false, null, false)]
        [DataRow("POINT(-96.7014 40.8146)", true, null, false)]
        [DataRow("POINT(-96.7014 40.8146)", true, "true", false)]
        [DataRow("POINT(-96.7014 40.8146)", true, false, false)]
        [DataRow("POINT(-96.7014 40.8146)", true, true, true)]
        public void Map_GeometryToString(string geometryString, bool hasKey, object key, bool? isGeoJsonExpected)
        {
            Geometry geometry = null;
            string expectedResult = null;
            if (geometryString != null)
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var reader = new WKTReader(geometryFactory.GeometryServices);
                geometry = reader.Read(geometryString);
                expectedResult = isGeoJsonExpected.Value ? geometry.AsGeoJson() : geometry.AsText();
            }

            Action<IMappingOperationOptions> mappingOperationsAction = hasKey ? a => a.Items.Add(ApiProfile.GeometryConversionKey, key) : null;

            var result = geometry.Map<string>(mappingOperationsAction);
            result.Should().Be(expectedResult);
        }
    }
}
