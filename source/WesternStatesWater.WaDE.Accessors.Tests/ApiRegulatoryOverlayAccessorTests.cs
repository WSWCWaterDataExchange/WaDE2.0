﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telerik.JustMock;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ApiRegulatoryOverlayAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task GetRegulatoryReportingUnitsAsync_Filters_None()
        {
            var configuration = Configuration.GetConfiguration();
            RegulatoryReportingUnitsFact regulatoryReportingUnitsFact;
            RegulatoryOverlayDim regulatoryOverlayDim;
            ReportingUnitsDim reportingUnitsDim;
            using (var db = new WaDEContext(configuration))
            {
                regulatoryOverlayDim = await RegulatoryOverlayDimBuilder.Load(db);
                reportingUnitsDim = await ReportingUnitsDimBuilder.Load(db);
                regulatoryReportingUnitsFact = await RegulatoryReportingUnitsFactBuilder.Load(db,
                    new RegulatoryReportingUnitsFactBuilderOptions
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

        [DataTestMethod]
        [DataRow("2022-02-24", null, null, true)]
        [DataRow("2022-02-24", "2022-02-23", null, true)]
        [DataRow("2022-02-24", "2022-02-24", null, true)]
        [DataRow("2022-02-24", "2022-02-25", null, false)]
        [DataRow("2022-02-24", null, "2022-02-23", false)]
        [DataRow("2022-02-24", null, "2022-02-24", true)]
        [DataRow("2022-02-24", null, "2022-02-25", true)]
        [DataRow("2022-02-22", "2022-02-23", "2022-02-25", false)]
        [DataRow("2022-02-23", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-24", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-25", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-26", "2022-02-23", "2022-02-25", false)]
        public async Task GetRegulatoryReportingUnitsAsync_Filters_DataPublicationDate(string dataPublicationDate,
            string startDataPublicationDate, string endDataPublicationDate, bool shouldMatch)
        {
            var configuration = Configuration.GetConfiguration();

            RegulatoryReportingUnitsFact regulatoryReportingUnitsFact;
            RegulatoryOverlayDim regulatoryOverlayDim;
            ReportingUnitsDim reportingUnitsDim;
            DateDim publicationDate;
            using (var db = new WaDEContext(configuration))
            {
                regulatoryOverlayDim = await RegulatoryOverlayDimBuilder.Load(db);
                reportingUnitsDim = await ReportingUnitsDimBuilder.Load(db);
                publicationDate = await DateDimBuilder.Load(db);
                publicationDate.Date = DateTime.Parse(dataPublicationDate);

                regulatoryReportingUnitsFact = await RegulatoryReportingUnitsFactBuilder.Load(db,
                    new RegulatoryReportingUnitsFactBuilderOptions
                    {
                        RegulatoryOverlay = regulatoryOverlayDim,
                        ReportingUnits = reportingUnitsDim,
                        DataPublicationDate = publicationDate
                    });
            }

            var filters = new RegulatoryOverlayFilters
            {
                StartDataPublicationDate =
                    startDataPublicationDate == null ? null : DateTime.Parse(startDataPublicationDate),
                EndDataPublicationDate = endDataPublicationDate == null ? null : DateTime.Parse(endDataPublicationDate),
            };

            var sut = CreateRegulatoryOverlayAccessor();
            var result = await sut.GetRegulatoryReportingUnitsAsync(filters, 0, int.MaxValue);

            if (shouldMatch)
            {
                result.TotalRegulatoryReportingUnitsCount.Should().Be(1);
                result.Organizations.Should().HaveCount(1);

                var org = result.Organizations.Single();
                org.ReportingUnitsRegulatory.Should().HaveCount(1);
                org.ReportingUnitsRegulatory[0].ReportingUnitUUID.Should().Be(reportingUnitsDim.ReportingUnitUuid);
                org.RegulatoryOverlays.Should().HaveCount(1);
                org.RegulatoryOverlays[0].RegulatoryOverlayUUID.Should().Be(regulatoryOverlayDim.RegulatoryOverlayUuid);
            }
            else
            {
                result.TotalRegulatoryReportingUnitsCount.Should().Be(0);
                result.Organizations.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task GetOverlayMetadata_ReturnsBoundaryBox()
        {
            var response = await CreateRegulatoryOverlayAccessor().GetOverlayMetadata();
            response.BoundaryBox.MinX.Should().Be(-180);
            response.BoundaryBox.MinY.Should().Be(18);
            response.BoundaryBox.MaxX.Should().Be(-93);
            response.BoundaryBox.MaxY.Should().Be(72);
            response.BoundaryBox.Crs.Should().Be("http://www.opengis.net/def/crs/OGC/1.3/CRS84");
        }

        private IRegulatoryOverlayAccessor CreateRegulatoryOverlayAccessor()
        {
            return new RegulatoryOverlayAccessor(
                Configuration.GetConfiguration(), 
                LoggerFactory,
                Mock.Create<IAccessorRequestHandlerResolver>());
        }
    }
}