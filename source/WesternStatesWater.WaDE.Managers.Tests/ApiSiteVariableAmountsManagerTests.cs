using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Api;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Managers.Tests
{
    [TestClass]
    public class ApiSiteVariableAmountsManagerTests
    {
        private readonly ISiteVariableAmountsAccessor SiteVariableAmountsAccessorMock = Mock.Create<ISiteVariableAmountsAccessor>(Behavior.Strict);

        [DataTestMethod]
        [DataRow(null, GeometryFormat.Wkt, null)]
        [DataRow(null, GeometryFormat.GeoJson, null)]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.Wkt, "POINT (-96.7014 40.8146)")]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.GeoJson, "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}")]
        public async Task GetSiteAllocationAmountsAsync_SiteGeometries(string geometryString, GeometryFormat geometryFormat, string expectedResultString)
        {
            var accessorResult = SiteVariableAmountsBuilder.Create();
            accessorResult.Organizations.First().SiteVariableAmounts[0].SiteGeometry = GeometryExtensions.GetGeometryByWkt(geometryString);
            SiteVariableAmountsAccessorMock.Arrange(a => a.GetSiteVariableAmountsAsync(Arg.IsAny<Accessors.Contracts.Api.SiteVariableAmountsFilters>(), 0, 1))
                                       .Returns(Task.FromResult(accessorResult));
            var sut = CreateSiteVariableAmountsManager();
            var result = await sut.GetSiteVariableAmountsAsync(new Contracts.Api.SiteVariableAmountsFilters(), 0, 1, geometryFormat);
            result.Should().NotBeNull();

            var resultGeometry = result.Organizations.First().SiteVariableAmounts[0].SiteGeometry;
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
        public async Task GetSiteAllocationAmountsAsync_WaterSourceGeometry(string geometryString, GeometryFormat geometryFormat, string expectedResultString)
        {
            var accessorResult = SiteVariableAmountsBuilder.Create();
            accessorResult.Organizations.First().WaterSources[0].WaterSourceGeometry = GeometryExtensions.GetGeometryByWkt(geometryString);
            SiteVariableAmountsAccessorMock.Arrange(a => a.GetSiteVariableAmountsAsync(Arg.IsAny<Accessors.Contracts.Api.SiteVariableAmountsFilters>(), 0, 1))
                                       .Returns(Task.FromResult(accessorResult));
            var sut = CreateSiteVariableAmountsManager();
            var result = await sut.GetSiteVariableAmountsAsync(new Contracts.Api.SiteVariableAmountsFilters(), 0, 1, geometryFormat);
            result.Should().NotBeNull();

            var resultGeometry = result.Organizations.First().WaterSources[0].WaterSourceGeometry;
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

        private ISiteVariableAmountsManager CreateSiteVariableAmountsManager()
        {
            return new SiteVariableAmountsManager(SiteVariableAmountsAccessorMock);
        }
    }
}
