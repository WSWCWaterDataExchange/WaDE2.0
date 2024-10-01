using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WaDEApiFunctions.v1;
using WesternStatesWater.WaDE.Contracts.Api;

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

            var context = Mock.Create<FunctionContext>();
            var request = new FakeHttpRequestData(context, new Uri($"http://localhost?VariableCV={faker.Random.Word()}&geoFormat={formatString}"));

            var sut = CreateAggregatedAmountsFunction();
            var result = await sut.Run(request, NullLogger.Instance);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            AggregatedAmountsManagerMock.Assert(a => a.GetAggregatedAmountsAsync(Arg.IsAny<AggregatedAmountsFilters>(), 0, 1000, expectedGeometryFormat), Occurs.Once());
        }

        [DataTestMethod]
        [DataRow(null, HttpStatusCode.BadRequest)]
        [DataRow("", HttpStatusCode.BadRequest)]
        [DataRow(" ", HttpStatusCode.BadRequest)]
        [DataRow("\t", HttpStatusCode.BadRequest)]
        [DataRow("good one", HttpStatusCode.OK)]
        public async Task Run_VariableCV(string variableCv, HttpStatusCode expectedHttpStatusCode)
        {
            AggregatedAmountsManagerMock.Arrange(a => a.GetAggregatedAmountsAsync(Arg.IsAny<AggregatedAmountsFilters>(), 0, 1000, GeometryFormat.Wkt))
                .Returns(Task.FromResult(new AggregatedAmounts()));

            var context = Mock.Create<FunctionContext>();
            var request = new FakeHttpRequestData(context, new Uri($"http://localhost?VariableCV={variableCv}"));

            var sut = CreateAggregatedAmountsFunction();
            var result = await sut.Run(request, NullLogger.Instance);
            result.StatusCode.Should().Be(expectedHttpStatusCode);

            if (expectedHttpStatusCode == HttpStatusCode.BadRequest)
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