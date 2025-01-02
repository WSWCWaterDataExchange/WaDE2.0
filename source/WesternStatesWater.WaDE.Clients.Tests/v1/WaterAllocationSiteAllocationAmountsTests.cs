using System;
using System.Net;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WaDEApiFunctions.v1;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

namespace WesternStatesWater.WaDE.Clients.Tests.v1
{
    [TestClass]
    public class WaterAllocationSiteAllocationAmountsTests : FunctionTestBase
    {
        private IWaterResourceManager _waterResourceManagerMock = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _waterResourceManagerMock = Mock.Create<IWaterResourceManager>(Behavior.Strict);
        }

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

            _waterResourceManagerMock
                .Arrange(mgr => mgr.Load<SiteAllocationAmountsSearchRequest, SiteAllocationAmountsSearchResponse>(
                    Arg.Matches<SiteAllocationAmountsSearchRequest>(req =>
                        req.StartIndex == 0 &&
                        req.RecordCount == 1000 &&
                        req.OutputGeometryFormat == expectedGeometryFormat
                    )))
                .ReturnsAsync(new SiteAllocationAmountsSearchResponse { WaterAllocations = new WaterAllocations() });

            var context = Mock.Create<FunctionContext>();
            var request = new FakeHttpRequestData(context, new Uri($"http://localhost?SiteUUID={faker.Random.Uuid()}&geoFormat={formatString}"));

            var sut = CreateSiteAllocationAmountsFunction();
            var result = await sut.Run(request);
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            _waterResourceManagerMock.Assert(a =>
                a.Load<SiteAllocationAmountsSearchRequest, SiteAllocationAmountsSearchResponse>(
                    Arg.Matches<SiteAllocationAmountsSearchRequest>(req =>
                        req.StartIndex == 0 &&
                        req.RecordCount == 1000 &&
                        req.OutputGeometryFormat == expectedGeometryFormat
                    )), Occurs.Once());
        }

        [DataTestMethod]
        [DataRow(null, HttpStatusCode.BadRequest)]
        [DataRow("", HttpStatusCode.BadRequest)]
        [DataRow(" ", HttpStatusCode.BadRequest)]
        [DataRow("\t", HttpStatusCode.BadRequest)]
        [DataRow("good one", HttpStatusCode.OK)]
        public async Task Run_SiteUuid(string siteUuid, HttpStatusCode expectedHttpStatusCode)
        {
            _waterResourceManagerMock
                .Arrange(mgr => mgr.Load<SiteAllocationAmountsSearchRequest, SiteAllocationAmountsSearchResponse>(
                    Arg.Matches<SiteAllocationAmountsSearchRequest>(req =>
                        req.StartIndex == 0 &&
                        req.RecordCount == 1000 &&
                        req.OutputGeometryFormat == GeometryFormat.Wkt
                    )))
                .ReturnsAsync(new SiteAllocationAmountsSearchResponse { WaterAllocations = new WaterAllocations() });

            var context = Mock.Create<FunctionContext>();
            var request = new FakeHttpRequestData(context, new Uri($"http://localhost?SiteUUID={siteUuid}"));

            var sut = CreateSiteAllocationAmountsFunction();
            var result = await sut.Run(request);
            result.StatusCode.Should().Be(expectedHttpStatusCode);

            if (expectedHttpStatusCode == HttpStatusCode.BadRequest)
            {
                _waterResourceManagerMock.Assert(a =>
                    a.Load<SiteAllocationAmountsSearchRequest, SiteAllocationAmountsSearchResponse>(
                        Arg.Matches<SiteAllocationAmountsSearchRequest>(req =>
                            req.StartIndex == 0 &&
                            req.RecordCount == 1000 &&
                            req.OutputGeometryFormat == GeometryFormat.Wkt
                        )), Occurs.Never());
            }
            else
            {
                _waterResourceManagerMock.Assert(a =>
                    a.Load<SiteAllocationAmountsSearchRequest, SiteAllocationAmountsSearchResponse>(
                        Arg.Matches<SiteAllocationAmountsSearchRequest>(req =>
                            req.Filters.SiteUuid == siteUuid &&
                            req.StartIndex == 0 &&
                            req.RecordCount == 1000 &&
                            req.OutputGeometryFormat == GeometryFormat.Wkt
                        )), Occurs.Once());
            }
        }

        private WaterAllocation_SiteAllocationAmounts CreateSiteAllocationAmountsFunction()
        {
            return new WaterAllocation_SiteAllocationAmounts(
                _waterResourceManagerMock,
                CreateLogger<WaterAllocation_SiteAllocationAmounts>()
            );
        }
    }
}