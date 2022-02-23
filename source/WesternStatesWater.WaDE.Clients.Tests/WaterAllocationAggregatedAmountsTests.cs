using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaDEApiFunctions.v1;
using WesternStatesWater.WaDE.Contracts.Api;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Bogus;
using System.Collections.Generic;
using System;

namespace WesternStatesWater.WaDE.Clients.Tests
{
    [TestClass]
    public class WaterAllocationAggregatedAmountsTests
    {
        private readonly IAggregatedAmountsManager AggregatedAmountsManagerMock = Mock.Create<IAggregatedAmountsManager>(Behavior.Strict);

        [DataTestMethod]
        [DataRow(null, GeometryFormat.Wkt)]
        [DataRow("", GeometryFormat.Wkt)]
        [DataRow(" ", GeometryFormat.Wkt)]
        [DataRow("\t", GeometryFormat.Wkt)]
        [DataRow("wkt", GeometryFormat.Wkt)]
        [DataRow("WKT", GeometryFormat.Wkt)]
        [DataRow("wKt", GeometryFormat.Wkt)]
        [DataRow("0", GeometryFormat.Wkt)]
        [DataRow("banana", GeometryFormat.Wkt)]
        [DataRow("geojson", GeometryFormat.GeoJson)]
        [DataRow("GEOJSON", GeometryFormat.GeoJson)]
        [DataRow("GeoJson", GeometryFormat.GeoJson)]
        [DataRow("1", GeometryFormat.GeoJson)]
        public async Task Run_GeometryFormat(string formatString, GeometryFormat expectedGeometryFormat)
        {
            var faker = new Faker();
            AggregatedAmountsManagerMock.Arrange(a => a.GetAggregatedAmountsAsync(Arg.IsAny<AggregatedAmountsFilters>(), 0, 1000, expectedGeometryFormat))
                                      .Returns(Task.FromResult(new AggregatedAmounts()));

            var httpContext = new DefaultHttpContext();
            var queryParams = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
            {
                { "VariableCV", faker.Random.Word() },
                { "geoFormat", formatString }
            };
            httpContext.Request.Query = new QueryCollection(queryParams);
            var sut = CreateAggregatedAmountsFunction();
            var result = await sut.Run(httpContext.Request, NullLogger.Instance);
            result.Should().BeOfType<JsonResult>();

            AggregatedAmountsManagerMock.Assert(a => a.GetAggregatedAmountsAsync(Arg.IsAny<AggregatedAmountsFilters>(), 0, 1000, expectedGeometryFormat), Occurs.Once());
        }

        [DataTestMethod]
        [DataRow(null, typeof(BadRequestObjectResult))]
        [DataRow("", typeof(BadRequestObjectResult))]
        [DataRow(" ", typeof(BadRequestObjectResult))]
        [DataRow("\t", typeof(BadRequestObjectResult))]
        [DataRow("good one", typeof(JsonResult))]
        public async Task Run_VariableCV(string variableCv, Type expectedType)
        {
            AggregatedAmountsManagerMock.Arrange(a => a.GetAggregatedAmountsAsync(Arg.IsAny<AggregatedAmountsFilters>(), 0, 1000, GeometryFormat.Wkt))
                                      .Returns(Task.FromResult(new AggregatedAmounts()));

            var httpContext = new DefaultHttpContext();
            var queryParams = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
            {
                { "VariableCV", variableCv }
            };
            httpContext.Request.Query = new QueryCollection(queryParams);
            var sut = CreateAggregatedAmountsFunction();
            var result = await sut.Run(httpContext.Request, NullLogger.Instance);
            result.Should().BeOfType(expectedType);

            if (expectedType == typeof(BadRequestObjectResult))
            {
                AggregatedAmountsManagerMock.Assert(a => a.GetAggregatedAmountsAsync(Arg.IsAny<AggregatedAmountsFilters>(), 0, 1000, GeometryFormat.Wkt), Occurs.Never());
            }
            else
            {
                AggregatedAmountsManagerMock.Assert(a => a.GetAggregatedAmountsAsync(Arg.IsAny<AggregatedAmountsFilters>(), 0, 1000, GeometryFormat.Wkt), Occurs.Once());
                AggregatedAmountsManagerMock.Assert(a => a.GetAggregatedAmountsAsync(Arg.Matches<AggregatedAmountsFilters>(a => a.VariableCV == variableCv), 0, 1000, GeometryFormat.Wkt), Occurs.Once());
            }
        }

        private WaterAllocation_AggregatedAmounts CreateAggregatedAmountsFunction()
        {
            return new WaterAllocation_AggregatedAmounts(AggregatedAmountsManagerMock);
        }
    }
}