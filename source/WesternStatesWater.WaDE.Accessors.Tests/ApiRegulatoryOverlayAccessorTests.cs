using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ApiRegulatoryOverlayAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task GetRegulatoryReportingUnitsAsync_NoFilters()
        {
            var configuration = Configuration.GetConfiguration();
            RegulatoryReportingUnitsFact regulatoryReportingUnitsFact;
            RegulatoryOverlayDim regulatoryOverlayDim;
            ReportingUnitsDim reportingUnitsDim;
            using (var db = new WaDEContext(configuration))
            {
                regulatoryOverlayDim = await RegulatoryOverlayDimBuilder.Load(db);
                reportingUnitsDim = await ReportingUnitsDimBuilder.Load(db);
                regulatoryReportingUnitsFact = await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
                {
                    RegulatoryOverlay = regulatoryOverlayDim,
                    ReportingUnits = reportingUnitsDim
                });
            }

            var filters = new RegulatoryOverlayFilters();

            var sut = CreateRegulatoryOverlayAccessor();
            var result = await sut.GetRegulatoryReportingUnitsAsync(filters, 0, int.MaxValue);

            result.TotalRegulatoryReportingUnitsCount.Should().Be(1);
            result.Organizations.Should().HaveCount(1);

            var org = result.Organizations.Single();
            org.OrganizationId.Should().Be(regulatoryReportingUnitsFact.OrganizationId);
            org.ReportingUnitsRegulatory.Should().HaveCount(1);
            org.ReportingUnitsRegulatory[0].ReportingUnitUUID.Should().Be(reportingUnitsDim.ReportingUnitUuid);

            org.RegulatoryOverlays.Should().HaveCount(1);
            org.RegulatoryOverlays[0].RegulatoryOverlayUUID.Should().Be(regulatoryOverlayDim.RegulatoryOverlayUuid);
        }

        private IRegulatoryOverlayAccessor CreateRegulatoryOverlayAccessor()
        {
            return new RegulatoryOverlayAccessor(Configuration.GetConfiguration(), LoggerFactory);
        }
    }
}