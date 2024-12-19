using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Handlers;

[TestClass]
public class OverlaySearchHandlerTests : DbTestBase
{
    [TestMethod]
    public async Task Handler_LimitSet_ReturnsCorrectAmount()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        for (var i = 0; i < 5; i++)
        {
            await ReportingUnitsDimBuilder.Load(db);
        }

        var request = new OverlaySearchRequest
        {
            Limit = 3
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(3);
    }

    [TestMethod]
    public async Task Handler_LastKeySet_PagesNextSet()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        List<ReportingUnitsDim> overlays = new();
        for (var i = 0; i < 10; i++)
        {
            overlays.Add(await ReportingUnitsDimBuilder.Load(db));
        }

        overlays.Sort((x, y) =>
            string.Compare(x.ReportingUnitUuid, y.ReportingUnitUuid, StringComparison.Ordinal));

        var request = new OverlaySearchRequest
        {
            Limit = 3,
            LastKey = overlays[2].ReportingUnitUuid
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(3);
        response.Overlays.Select(a => a.ReportingUnitUUID).Should().BeEquivalentTo(
            overlays.Skip(3).Take(3).Select(a => a.ReportingUnitUuid));
    }

    [TestMethod]
    public async Task Handler_FilterReportingUnitUuid_ReturnsRequestedOverlays()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var overlayA = await ReportingUnitsDimBuilder.Load(db);
        var overlayB = await ReportingUnitsDimBuilder.Load(db);
        var overlayC = await ReportingUnitsDimBuilder.Load(db);

        var request = new OverlaySearchRequest
        {
            OverlayUuids = [overlayA.ReportingUnitUuid, overlayB.ReportingUnitUuid],
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(2);
        response.Overlays.Select(a => a.ReportingUnitUUID).Should().BeEquivalentTo(
            overlayA.ReportingUnitUuid, overlayB.ReportingUnitUuid);
    }

    [TestMethod]
    public async Task Handler_FilterOverlayUuids_ReturnsRelatedOverlays()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var areaA = await ReportingUnitsDimBuilder.Load(db);
        var areaB = await ReportingUnitsDimBuilder.Load(db);
        
        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayB = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayC = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayD = await RegulatoryOverlayDimBuilder.Load(db);
        
        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaA
        });
        
        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayB,
            ReportingUnits = areaA
        });
        
        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayC,
            ReportingUnits = areaB
        });
        
        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayD,
            ReportingUnits = areaB
        });
        
        var request = new OverlaySearchRequest
        {
            OverlayUuids = [overlayA.RegulatoryOverlayUuid, overlayB.RegulatoryOverlayUuid],
            Limit = 10
        };
        
        // Act
        var response = await ExecuteHandler(request);
        
        // Assert
        response.Overlays.Should().HaveCount(1);
        response.Overlays.Select(a => a.ReportingUnitUUID).Should().BeEquivalentTo(
            areaA.ReportingUnitUuid);
    }

    [TestMethod]
    public async Task Handler_FilterSiteUuids_ReturnsRelatedOverlays()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);

        var areaA = await ReportingUnitsDimBuilder.Load(db);
        var areaB = await ReportingUnitsDimBuilder.Load(db);
        
        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayB = await RegulatoryOverlayDimBuilder.Load(db);

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaA
        });
        
        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayB,
            ReportingUnits = areaB
        });
        
        await RegulatoryOverlayBridgeSitesFactBuilder.Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            RegulatoryOverlayDim = overlayA
        });
        
        await RegulatoryOverlayBridgeSitesFactBuilder.Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            RegulatoryOverlayDim = overlayB
        });
        
        await RegulatoryOverlayBridgeSitesFactBuilder.Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions());

        var request = new OverlaySearchRequest
        {
            SiteUuids = [siteA.SiteUuid, siteB.SiteUuid],
            Limit = 10
        };
        
        // Act
        var response = await ExecuteHandler(request);
        
        // Assert
        response.Overlays.Should().HaveCount(2);
        response.Overlays.Select(a => a.ReportingUnitUUID).Should().BeEquivalentTo(
            overlayA.RegulatoryOverlayUuid, overlayB.RegulatoryOverlayUuid);
    }

    private async Task<OverlaySearchResponse> ExecuteHandler(OverlaySearchRequest request)
    {
        var handler = new OverlaySearchHandler(Configuration.GetConfiguration());
        return await handler.Handle(request);
    }
}