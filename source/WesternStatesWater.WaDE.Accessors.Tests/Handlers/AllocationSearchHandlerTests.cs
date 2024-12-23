using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Handlers;

[TestClass]
public class AllocationSearchHandlerTests : DbTestBase
{
    [TestMethod]
    public async Task Handler_LimitSet_ReturnsCorrectAmount()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        for (var i = 0; i < 5; i++)
        {
            await AllocationAmountsFactBuilder.Load(db);
        }

        var request = new AllocationSearchRequest
        {
            Limit = 3
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Allocations.Should().HaveCount(3);
    }

    [TestMethod]
    public async Task Handler_LastKeySet_PagesNextSet()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        List<AllocationAmountsFact> allocations = new();
        for (var i = 0; i < 10; i++)
        {
            allocations.Add(await AllocationAmountsFactBuilder.Load(db));
        }

        allocations.Sort((x, y) => String.Compare(x.AllocationUUID, y.AllocationUUID, StringComparison.Ordinal));

        var request = new AllocationSearchRequest
        {
            Limit = 3,
            LastKey = allocations[2].AllocationUUID
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Allocations.Should().HaveCount(3);
        response.Allocations.Select(a => a.AllocationUUID).Should().BeEquivalentTo(
            allocations[3].AllocationUUID,
            allocations[4].AllocationUUID,
            allocations[5].AllocationUUID);
    }

    [TestMethod]
    public async Task Handler_FilterAllocationUuid_ReturnsRequestedAllocations()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);

        var allocationA = await AllocationAmountsFactBuilder.Load(db);

        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = allocationA
        });

        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            AllocationAmountsFact = allocationA
        });

        var request = new AllocationSearchRequest
        {
            AllocationUuid = [allocationA.AllocationUUID],
            Limit = 10
        };
        var response = await ExecuteHandler(request);

        response.Allocations.Should().HaveCount(1);
        response.Allocations.Select(alloc => alloc.AllocationUUID).Should().BeEquivalentTo(allocationA.AllocationUUID);
    }

    [TestMethod]
    public async Task Handler_FilterSiteUuids_ReturnsRelatedAllocations()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        var siteC = await SitesDimBuilder.Load(db);

        // Allocation with a site
        var allocation1 = await AllocationAmountsFactBuilder.Load(db);
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = allocation1
        });

        // Allocation with no site
        await AllocationAmountsFactBuilder.Load(db);

        // Allocation with multiple sites
        var allocation2 = await AllocationAmountsFactBuilder.Load(db);
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = allocation2
        });
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            AllocationAmountsFact = allocation2
        });

        var request = new AllocationSearchRequest
        {
            SiteUuid = [siteA.SiteUuid, siteB.SiteUuid, siteC.SiteUuid],
            Limit = 10
        };
        var response = await ExecuteHandler(request);

        response.Allocations.Select(alloc => alloc.AllocationUUID).Should()
            .BeEquivalentTo(allocation1.AllocationUUID, allocation2.AllocationUUID);
        response.Allocations.First(alloc => alloc.AllocationUUID == allocation1.AllocationUUID).SitesUUIDs.Should()
            .BeEquivalentTo(siteA.SiteUuid);
        response.Allocations.First(alloc => alloc.AllocationUUID == allocation2.AllocationUUID).SitesUUIDs.Should()
            .BeEquivalentTo(siteA.SiteUuid, siteB.SiteUuid);
    }
    
    private async Task<AllocationSearchResponse> ExecuteHandler(AllocationSearchRequest request)
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var handler = new AllocationSearchHandler(Configuration.GetConfiguration());
        return await handler.Handle(request);
    }
}