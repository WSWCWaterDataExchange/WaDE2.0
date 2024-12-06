using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Managers.Tests.WaterResourceManager
{
    [TestClass]
    public class ApiSiteVariableAmountsManagerTests : WaterResourceManagerTestsBase
    {
        [DataTestMethod]
        [DataRow(null, GeometryFormat.Wkt, null)]
        [DataRow(null, GeometryFormat.GeoJson, null)]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.Wkt, "POINT (-96.7014 40.8146)")]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.GeoJson, "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}")]
        public async Task GetSiteAllocationAmountsAsync_Site_SiteGeometries(string geometryString, GeometryFormat geometryFormat, string expectedResultString)
        {
            var accessorResult = SiteVariableAmountsBuilder.Create();
            accessorResult.Organizations.First().Sites[0].SiteGeometry = GeometryExtensions.GetGeometryByWkt(geometryString);
            SiteVariableAmountsAccessorMock.Arrange(a => a.GetSiteVariableAmountsAsync(Arg.IsAny<Accessors.Contracts.Api.SiteVariableAmountsFilters>(), 0, 1))
                                       .Returns(Task.FromResult(accessorResult));
            var sut = CreateSiteVariableAmountsManager();
            var result = await sut.GetSiteVariableAmountsAsync(new Contracts.Api.SiteVariableAmountsFilters(), 0, 1, geometryFormat);
            result.Should().NotBeNull();

            var resultGeometry = result.Organizations.First().Sites[0].SiteGeometry;
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
        public async Task GetSiteAllocationAmountsAsync_WaterSource_WaterSourceGeometry(string geometryString, GeometryFormat geometryFormat, string expectedResultString)
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
            return CreateWaterResourceManager();
        }
    }
}
