using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.IO;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Accessors.Mapping;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Handlers;

[TestClass]
public class AllocationSearchHandlerTests : DbTestBase
{
    [DataTestMethod]
    [DataRow(5)]
    [DataRow(10)]
    public async Task Handler_LimitSet_EndOfRecordsNoLastUuid(int limit)
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        for (var i = 0; i < 5; i++)
        {
            await AllocationAmountsFactBuilder.Load(db);
        }

        var request = new AllocationSearchRequest
        {
            Limit = limit
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Allocations.Should().HaveCount(5);
        response.LastUuid.Should().BeNull();
    }

    [TestMethod]
    public async Task Handler_LimitSet_ReturnsCorrectAmount()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        List<AllocationAmountsFact> dbAllocations = new();
        for (var i = 0; i < 5; i++)
        {
            dbAllocations.Add(await AllocationAmountsFactBuilder.Load(db));
        }

        var request = new AllocationSearchRequest
        {
            Limit = 3
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Allocations.Should().HaveCount(3);
        response.LastUuid.Should()
            .Be(dbAllocations.OrderBy(a => a.AllocationUUID).Select(a => a.AllocationUUID).ElementAt(2));
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

    [TestMethod]
    public async Task Handler_FilterBoundary_ShouldReturnAllocationWhoHasASiteIntersectingFilter()
    {
        // Visualize sites: https://wktmap.com/?3a6d3a8a
        var wktReader = new WKTReader();
        var pointA = wktReader.Read("POINT (-113.08223904518697 39.07881679978712)");
        var pointB = wktReader.Read("POINT (-111.88412458869138 39.24952488971968)");
        var pointC = wktReader.Read("POINT (-111.27380883752618 39.2921796551922)");
        var polygonA =
            wktReader.Read(
                "POLYGON ((-112.04138848167361 39.40747600260892, -112.04138848167361 39.364864033742805, -111.99726180323547 39.364864033742805, -111.99726180323547 39.40747600260892, -112.04138848167361 39.40747600260892))");

        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var site1 = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            SitePoint = pointA
        });
        var site2 = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            SitePoint = pointB
        });
        var site3 = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            SitePoint = pointC
        });
        var site4 = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = polygonA
        });

        var allocationOutsideBoundary = await AllocationAmountsFactBuilder.Load(db);
        var allocationWithOneSiteInBoundary = await AllocationAmountsFactBuilder.Load(db);
        var allocationWithSiteInBoundary = await AllocationAmountsFactBuilder.Load(db);

        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = site1,
            AllocationAmountsFact = allocationWithOneSiteInBoundary
        });
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = site2,
            AllocationAmountsFact = allocationWithOneSiteInBoundary
        });
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = site3,
            AllocationAmountsFact = allocationOutsideBoundary
        });
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = site4,
            AllocationAmountsFact = allocationWithSiteInBoundary
        });

        // Filter Boundary visualizer: https://wktmap.com/?e32407a4
        // Visualize all coordinates: https://wktmap.com/?afc484da
        var request = new AllocationSearchRequest
        {
            GeometrySearch = new SpatialSearchCriteria
            {
                Geometry = wktReader.Read(
                    "POLYGON ((-112.08792189270426 39.44602478526929, -112.08792189270426 39.172494625547614, -111.73175117928515 39.172494625547614, -111.73175117928515 39.44602478526929, -112.08792189270426 39.44602478526929))"),
                SpatialRelationType = SpatialRelationType.Intersects
            },
            Limit = 5
        };

        var response = await ExecuteHandler(request);

        response.Allocations.Should().HaveCount(2);
        response.Allocations.Select(a => a.AllocationUUID).Should().BeEquivalentTo(
            allocationWithOneSiteInBoundary.AllocationUUID,
            allocationWithSiteInBoundary.AllocationUUID);
    }

    [TestMethod]
    public async Task Handler_FilterWaterSourceTypes_ShouldReturnAllocationsWithWaterSourceType()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var waterSourceTypeA = await WaterSourceTypeBuilder.Load(db);
        var waterSourceTypeB = await WaterSourceTypeBuilder.Load(db);

        var waterSourceA = await WaterSourcesDimBuilder.Load(db, new WaterSourcesDimBuilderOptions
        {
            WaterSourceType = waterSourceTypeA
        });
        var waterSourceB = await WaterSourcesDimBuilder.Load(db, new WaterSourcesDimBuilderOptions
        {
            WaterSourceType = waterSourceTypeB
        });

        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);

        await WaterSourceBridgeSitesFactBuilder.Load(db,
            new WaterSourceBridgeSitesFactBuilderOptions
            {
                SitesDim = siteA,
                WaterSourcesDim = waterSourceA
            });
        await WaterSourceBridgeSitesFactBuilder.Load(db,
            new WaterSourceBridgeSitesFactBuilderOptions
            {
                SitesDim = siteB,
                WaterSourcesDim = waterSourceB
            });

        var allocationA = await AllocationAmountsFactBuilder.Load(db);
        var allocationB = await AllocationAmountsFactBuilder.Load(db);

        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = allocationA
        });
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            AllocationAmountsFact = allocationB
        });

        var request = new AllocationSearchRequest
        {
            WaterSourceTypes = [waterSourceTypeA.WaDEName],
            Limit = 10
        };

        var response = await ExecuteHandler(request);

        response.Allocations.Should().HaveCount(1);
        response.Allocations.Select(a => a.AllocationUUID).Should().BeEquivalentTo(allocationA.AllocationUUID);
        response.Allocations[0].WaterSources.Should()
            .BeEquivalentTo([waterSourceA.Map<WaterSourceSummary>()]);
    }

    [TestMethod]
    public async Task Handler_FilterBeneficialUses_ShouldReturnAllocationsWithBeneficialUses()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        // Create three beneficial uses, but have two share a common WaDEName.
        var beneficialUseA = await BeneficalUsesBuilder.Load(db, new BeneficalUsesBuilderOptions
        {
            WaDEName = "Irrigation"
        });
        var beneficialUseB = await BeneficalUsesBuilder.Load(db, new BeneficalUsesBuilderOptions
        {
            WaDEName = "Irrigation"
        });
        var beneficialUseC = await BeneficalUsesBuilder.Load(db, new BeneficalUsesBuilderOptions
        {
            WaDEName = "Municipal"
        });

        var allocationA = await AllocationAmountsFactBuilder.Load(db);
        var allocationB = await AllocationAmountsFactBuilder.Load(db);

        await AllocationBridgeBeneficialUsesFactBuilder.Load(db,
            new AllocationBridgeBeneficialUsesFactBuilderOptions
            {
                BeneficialUsesCv = beneficialUseA,
                AllocationAmountsFact = allocationA
            });
        await AllocationBridgeBeneficialUsesFactBuilder.Load(db,
            new AllocationBridgeBeneficialUsesFactBuilderOptions
            {
                BeneficialUsesCv = beneficialUseB,
                AllocationAmountsFact = allocationA
            });

        await AllocationBridgeBeneficialUsesFactBuilder.Load(db,
            new AllocationBridgeBeneficialUsesFactBuilderOptions
            {
                BeneficialUsesCv = beneficialUseC,
                AllocationAmountsFact = allocationB
            });

        var request = new AllocationSearchRequest
        {
            BeneficialUses = ["Irrigation"],
            Limit = 10
        };

        var response = await ExecuteHandler(request);

        response.Allocations.Should().HaveCount(1);
        response.Allocations.Select(a => a.AllocationUUID).Should().BeEquivalentTo(allocationA.AllocationUUID);
        response.Allocations[0].BeneficialUses.Should().BeEquivalentTo(beneficialUseA.WaDEName);
    }

    [TestMethod]
    public async Task Handler_FilterStates_ReturnsAllocationsInStates()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        // New state is created with each Site
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        var siteC = await SitesDimBuilder.Load(db);
        var siteD = await SitesDimBuilder.Load(db);

        // Ensure all states are unique
        string[] expectUnique =
        [
            siteA.StateCVNavigation.Name, siteB.StateCVNavigation.Name, siteC.StateCVNavigation.Name,
            siteD.StateCVNavigation.Name
        ];
        expectUnique.Should().OnlyHaveUniqueItems();

        var allocationA = await AllocationAmountsFactBuilder.Load(db);
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = allocationA
        });

        var allocationB = await AllocationAmountsFactBuilder.Load(db);
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            AllocationAmountsFact = allocationB
        });

        var allocationC = await AllocationAmountsFactBuilder.Load(db);
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteC,
            AllocationAmountsFact = allocationC
        });

        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteD,
            AllocationAmountsFact = allocationC
        });

        var request = new AllocationSearchRequest
        {
            States = [siteA.StateCv, siteC.StateCv],
            Limit = 10
        };
        var response = await ExecuteHandler(request);

        response.Allocations.Should().HaveCount(2);
        response.Allocations.First(alloc => alloc.AllocationUUID == allocationA.AllocationUUID).States.Should()
            .BeEquivalentTo(siteA.StateCv);
        response.Allocations.First(alloc => alloc.AllocationUUID == allocationC.AllocationUUID).States.Should()
            .BeEquivalentTo(siteC.StateCv, siteD.StateCv);
        response.Allocations.Select(alloc => alloc.AllocationUUID).Should()
            .BeEquivalentTo(allocationA.AllocationUUID, allocationC.AllocationUUID);
    }

    [DataTestMethod]
    [DataRow("2025-02-01", null, 4)]
    [DataRow(null, "2025-02-01", 2)]
    [DataRow("2025-02-01", "2025-02-28", 3)]
    public async Task Handler_FilterPriorityDate_ReturnsAllocationsInPriorityRange(string start, string end,
        int expectedMatchCount)
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
        {
            AllocationPriorityDate = await DateDimBuilder.Load(db, new DateDimBuilderOptions
            {
                Date = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            })
        });

        await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
        {
            AllocationPriorityDate = await DateDimBuilder.Load(db, new DateDimBuilderOptions
            {
                Date = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            })
        });
        await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
        {
            AllocationPriorityDate = await DateDimBuilder.Load(db, new DateDimBuilderOptions
            {
                Date = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc)
            })
        });
        await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
        {
            AllocationPriorityDate = await DateDimBuilder.Load(db, new DateDimBuilderOptions
            {
                Date = new DateTime(2025, 2, 28, 0, 0, 0, DateTimeKind.Utc)
            })
        });

        var x =await AllocationAmountsFactBuilder.Load(db, new AllocationAmountsFactBuilderOptions
        {
            AllocationPriorityDate = await DateDimBuilder.Load(db, new DateDimBuilderOptions
            {
                Date = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc)
            })
        });

        DateTime? startDate = start != null ? DateTime.Parse(start) : null;
        DateTime? endDate = end != null ? DateTime.Parse(end) : null;
        var request = new AllocationSearchRequest
        {
            PriorityDate = new DateRangeFilter
            {
                StartDate = startDate != null
                    ? new DateTimeOffset(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0,
                        TimeSpan.Zero)
                    : null,
                EndDate = endDate != null
                    ? new DateTimeOffset(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 0, 0, 0,
                        TimeSpan.Zero)
                    : null
            },
            Limit = 10
        };
        var response = await ExecuteHandler(request);

        response.Allocations.Should().HaveCount(expectedMatchCount);
    }

    private async Task<AllocationSearchResponse> ExecuteHandler(AllocationSearchRequest request)
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var handler = new AllocationSearchHandler(Configuration.GetConfiguration());
        return await handler.Handle(request);
    }
}