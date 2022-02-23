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
    public class ApiWaterAllocationManagerTests
    {
        private readonly IWaterAllocationAccessor WaterAllocationAccessorMock = Mock.Create<IWaterAllocationAccessor>(Behavior.Strict);

        [DataTestMethod]
        [DataRow(null, GeometryFormat.Wkt, null)]
        [DataRow(null, GeometryFormat.GeoJson, null)]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.Wkt, "POINT (-96.7014 40.8146)")]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.GeoJson, "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}")]
        public async Task GetSiteAllocationAmountsAsync_SiteGeometries(string geometryString, GeometryFormat geometryFormat, string expectedResult)
        {
            var accessorResult = WaterAllocationsBuilder.Create();
            accessorResult.Organizations.First().WaterAllocations[0].Sites[0].SiteGeometry = GeometryExtensions.GetGeometryByWkt(geometryString);
            WaterAllocationAccessorMock.Arrange(a => a.GetSiteAllocationAmountsAsync(Arg.IsAny<Accessors.Contracts.Api.SiteAllocationAmountsFilters>(), 0, 1))
                                       .Returns(Task.FromResult(accessorResult));
            var sut = CreateWaterAllocationManager();
            var result = await sut.GetSiteAllocationAmountsAsync(new Contracts.Api.SiteAllocationAmountsFilters(), 0, 1, geometryFormat);
            result.Should().NotBeNull();

            result.Organizations.First().WaterAllocations[0].Sites[0].SiteGeometry.Should().Be(expectedResult);
        }

        private IWaterAllocationManager CreateWaterAllocationManager()
        {
            return new WaterAllocationManager(WaterAllocationAccessorMock);
        }
    }
}
