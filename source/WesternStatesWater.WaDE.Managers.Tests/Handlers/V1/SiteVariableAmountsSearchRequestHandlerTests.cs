using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Managers.Api.Handlers.V1;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers.V1;

public class SiteVariableAmountsSearchRequestHandlerTests : HandlerTestsBase
{
        [DataTestMethod]
        [DataRow(null, GeometryFormat.Wkt, null)]
        [DataRow(null, GeometryFormat.GeoJson, null)]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.Wkt, "POINT (-96.7014 40.8146)")]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.GeoJson, "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}")]
        public async Task GetSiteVariableAmountsAsync_Site_SiteGeometries(string geometryString, GeometryFormat geometryFormat, string expectedResultString)
        {
            var accessorResult = SiteVariableAmountsBuilder.Create();
            accessorResult.Organizations.First().Sites[0].SiteGeometry = GeometryExtensions.GetGeometryByWkt(geometryString);
            SiteVariableAmountsAccessorMock.Arrange(a => a.GetSiteVariableAmountsAsync(Arg.IsAny<Accessors.Contracts.Api.SiteVariableAmountsFilters>(), 0, 1))
                                       .Returns(Task.FromResult(accessorResult));
            var sut = CreateHandler();

            var request = new SiteVariableAmountsSearchRequest
            {
                Filters = new SiteVariableAmountsFilters(),
                StartIndex = 0,
                RecordCount = 1,
                OutputGeometryFormat = geometryFormat
            };

            var result = await sut.Handle(request);
            result.Should().NotBeNull();

            var resultGeometry = result.SiteVariableAmounts.Organizations.First().Sites[0].SiteGeometry;
            if (expectedResultString == null)
            {
                resultGeometry.Should().BeNull();
            }
            else
            {
                var expectedResult = geometryFormat == GeometryFormat.Wkt ? expectedResultString : Newtonsoft.Json.JsonConvert.DeserializeObject(expectedResultString);
                resultGeometry.ToString().Should().Be(expectedResult.ToString());
            }
        }

        [DataTestMethod]
        [DataRow(null, GeometryFormat.Wkt, null)]
        [DataRow(null, GeometryFormat.GeoJson, null)]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.Wkt, "POINT (-96.7014 40.8146)")]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.GeoJson, "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}")]
        public async Task GetSiteVariableAmountsAsync_WaterSource_WaterSourceGeometry(string geometryString, GeometryFormat geometryFormat, string expectedResultString)
        {
            var accessorResult = SiteVariableAmountsBuilder.Create();
            accessorResult.Organizations.First().WaterSources[0].WaterSourceGeometry = GeometryExtensions.GetGeometryByWkt(geometryString);
            SiteVariableAmountsAccessorMock.Arrange(a => a.GetSiteVariableAmountsAsync(Arg.IsAny<Accessors.Contracts.Api.SiteVariableAmountsFilters>(), 0, 1))
                                       .Returns(Task.FromResult(accessorResult));
            var sut = CreateHandler();
            
            var request = new SiteVariableAmountsSearchRequest
            {
                Filters = new SiteVariableAmountsFilters(),
                StartIndex = 0,
                RecordCount = 1,
                OutputGeometryFormat = geometryFormat
            };
            
            var result = await sut.Handle(request);
            
            result.Should().NotBeNull();

            var resultGeometry = result.SiteVariableAmounts.Organizations.First().WaterSources[0].WaterSourceGeometry;
            if (expectedResultString == null)
            {
                resultGeometry.Should().BeNull();
            }
            else
            {
                var expectedResult = geometryFormat == GeometryFormat.Wkt ? expectedResultString : Newtonsoft.Json.JsonConvert.DeserializeObject(expectedResultString);
                resultGeometry.ToString().Should().Be(expectedResult.ToString());
            }
        }

    private SiteVariableAmountsSearchRequestHandler CreateHandler()
    {
        return new SiteVariableAmountsSearchRequestHandler(SiteVariableAmountsAccessorMock);
    }
}