using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WaDEApiFunctions.v1;
using WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Clients.Tests
{
    [TestClass]
    public class WaterAllocationSiteAllocationAmountsTests
    {
        private readonly IWaterAllocationManager WaterAllocationManagerMock = Mock.Create<IWaterAllocationManager>(Behavior.Strict);

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
            WaterAllocationManagerMock.Arrange(a => a.GetSiteAllocationAmountsAsync(Arg.IsAny<SiteAllocationAmountsFilters>(), 0, 1000, expectedGeometryFormat))
                                      .Returns(Task.FromResult(new WaterAllocations()));

            var httpContext = new DefaultHttpContext();
            var queryParams = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
            {
                { "SiteUUID", faker.Random.Uuid().ToString() },
                { "geoFormat", formatString }
            };
            httpContext.Request.Query = new QueryCollection(queryParams);
            var sut = CreateSiteAllocationAmountsFunction();
            var result = await sut.Run(httpContext.Request, NullLogger.Instance);
            result.Should().BeOfType<JsonResult>();

            WaterAllocationManagerMock.Assert(a => a.GetSiteAllocationAmountsAsync(Arg.IsAny<SiteAllocationAmountsFilters>(), 0, 1000, expectedGeometryFormat), Occurs.Once());
        }

        [DataTestMethod]
        [DataRow(null, typeof(BadRequestObjectResult))]
        [DataRow("", typeof(BadRequestObjectResult))]
        [DataRow(" ", typeof(BadRequestObjectResult))]
        [DataRow("\t", typeof(BadRequestObjectResult))]
        [DataRow("good one", typeof(JsonResult))]
        public async Task Run_SiteUuid(string siteUuid, Type expectedType)
        {
            WaterAllocationManagerMock.Arrange(a => a.GetSiteAllocationAmountsAsync(Arg.IsAny<SiteAllocationAmountsFilters>(), 0, 1000, GeometryFormat.Wkt))
                                      .Returns(Task.FromResult(new WaterAllocations()));

            var httpContext = new DefaultHttpContext();
            var queryParams = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
            {
                { "SiteUUID", siteUuid }
            };
            httpContext.Request.Query = new QueryCollection(queryParams);
            var sut = CreateSiteAllocationAmountsFunction();
            var result = await sut.Run(httpContext.Request, NullLogger.Instance);
            result.Should().BeOfType(expectedType);

            if (expectedType == typeof(BadRequestObjectResult))
            {
                WaterAllocationManagerMock.Assert(a => a.GetSiteAllocationAmountsAsync(Arg.IsAny<SiteAllocationAmountsFilters>(), 0, 1000, GeometryFormat.Wkt), Occurs.Never());
            }
            else
            {
                WaterAllocationManagerMock.Assert(a => a.GetSiteAllocationAmountsAsync(Arg.IsAny<SiteAllocationAmountsFilters>(), 0, 1000, GeometryFormat.Wkt), Occurs.Once());
                WaterAllocationManagerMock.Assert(a => a.GetSiteAllocationAmountsAsync(Arg.Matches<SiteAllocationAmountsFilters>(a => a.SiteUuid == siteUuid), 0, 1000, GeometryFormat.Wkt), Occurs.Once());
            }
        }

        private WaterAllocation_SiteAllocationAmounts CreateSiteAllocationAmountsFunction()
        {
            return new WaterAllocation_SiteAllocationAmounts(WaterAllocationManagerMock);
        }
    }
}