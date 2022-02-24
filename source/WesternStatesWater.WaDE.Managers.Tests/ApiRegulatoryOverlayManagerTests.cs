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
    public class ApiRegulatoryOverlayManagerTests
    {
        private readonly IRegulatoryOverlayAccessor RegulatoryOverlayAccessorMock = Mock.Create<IRegulatoryOverlayAccessor>(Behavior.Strict);

        [DataTestMethod]
        [DataRow(null, GeometryFormat.Wkt, null)]
        [DataRow(null, GeometryFormat.GeoJson, null)]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.Wkt, "POINT (-96.7014 40.8146)")]
        [DataRow("POINT (-96.7014 40.8146)", GeometryFormat.GeoJson, "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}")]
        public async Task GetSiteAllocationAmountsAsync_SiteGeometries(string geometryString, GeometryFormat geometryFormat, string expectedResultString)
        {
            var accessorResult = RegulatoryReportingUnitsBuilder.Create();
            accessorResult.Organizations.First().ReportingUnitsRegulatory[0].Geometry = GeometryExtensions.GetGeometryByWkt(geometryString);
            RegulatoryOverlayAccessorMock.Arrange(a => a.GetRegulatoryReportingUnitsAsync(Arg.IsAny<Accessors.Contracts.Api.RegulatoryOverlayFilters>(), 0, 1))
                                       .Returns(Task.FromResult(accessorResult));
            var sut = CreateRegulatoryOverlayManager();
            var result = await sut.GetRegulatoryReportingUnitsAsync(new Contracts.Api.RegulatoryOverlayFilters(), 0, 1, geometryFormat);
            result.Should().NotBeNull();

            var resultGeometry = result.Organizations.First().ReportingUnitsRegulatory[0].Geometry;
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

        private IRegulatoryOverlayManager CreateRegulatoryOverlayManager()
        {
            return new RegulatoryOverlayManager(RegulatoryOverlayAccessorMock);
        }
    }
}
