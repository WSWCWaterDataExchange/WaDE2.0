using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
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
public class SiteSearchHandlerTests : DbTestBase
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
            await SitesDimBuilder.Load(db);
        }
        
        var request = new SiteSearchRequest
        {
            Limit = limit
        };
        
        // Act
        var response = await ExecuteHandler(request);
        
        // Assert
        response.Sites.Should().HaveCount(5);
        response.LastUuid.Should().BeNull();
    }
    
    [TestMethod]
    public async Task Handler_LimitSet_ReturnsCorrectAmount()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        
        List<SitesDim> dbSites = new();
        for (var i = 0; i < 5; i++)
        {
            dbSites.Add(await SitesDimBuilder.Load(db));
        }
        
        var request = new SiteSearchRequest
        {
            Limit = 3
        };
        
        // Act
        var response = await ExecuteHandler(request);
        
        // Assert
        response.Sites.Should().HaveCount(3);
        response.LastUuid.Should()
            .Be(dbSites.OrderBy(s => s.SiteUuid).Select(s => s.SiteUuid).ElementAt(2));
    }
    
    [TestMethod]
    public async Task SiteAccessor_GeometryFilter_ReturnsIntersectedSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var filterPolygon = CreateUtahPolygon();

        var wkt = new NetTopologySuite.IO.WKTReader();
        // Intersecting polygon
        var siteA = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            // https://wktmap.com/?28eb0eb1
            Geometry = wkt.Read(
                "POLYGON ((-114.400635 41.574361, -114.400635 42.204107, -113.57666 42.204107, -113.57666 41.574361, -114.400635 41.574361))")
        });

        // Points inside boundary
        var siteB = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = wkt.Read("POINT (-112.631836 39.010648)") // https://wktmap.com/?45c78c27
        });
        var siteC = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = wkt.Read("POINT (-111.928711 41.327326)") // https://wktmap.com/?23e9c33d
        });

        // Point outside boundary
        await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = wkt.Read("POINT (-115.026855 40.988192)")
        });
        
        var request = new SiteSearchRequest
        {
            GeometrySearch = new SpatialSearchCriteria
            {
                Geometry = filterPolygon,
                SpatialRelationType = SpatialRelationType.Intersects
            },
            Limit = 10
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(3);
        response.Sites.Select(s => s.SiteUuid).Should()
            .BeEquivalentTo(siteA.SiteUuid, siteB.SiteUuid, siteC.SiteUuid);
    }

    [TestMethod]
    public async Task SiteAccessor_Response_ShouldBeOrderedBySiteUuid()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        for (var i = 0; i < 5; i++)
        {
            await SitesDimBuilder.Load(db);
        }

        var request = new SiteSearchRequest
        {
            Limit = 3
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(3);
        response.Sites.Select(site => site.SiteUuid).Should().BeInAscendingOrder();
    }

    [TestMethod]
    public async Task SiteAccessor_NextPage_ShouldReturnNextSetofSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        List<SitesDim> sites = new();
        for (var i = 0; i < 10; i++)
        {
            sites.Add(await SitesDimBuilder.Load(db));
        }

        sites.Sort((x, y) => string.Compare(x.SiteUuid, y.SiteUuid, StringComparison.Ordinal));

        var request = new SiteSearchRequest
        {
            LastSiteUuid = sites[3].SiteUuid,
            Limit = 3
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(3);
        response.Sites.Select(s => s.SiteUuid).Should().BeEquivalentTo(
            sites[4].SiteUuid,
            sites[5].SiteUuid,
            sites[6].SiteUuid
        );
    }

    [TestMethod]
    public async Task Map()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var siteA = await SitesDimBuilder.Load(db);

        var podSite = await SitesDimBuilder.Load(db);
        var pouSite = await SitesDimBuilder.Load(db);
        
        await PODSiteToPOUSiteFactBuilder.Load(db, new PODSiteToPOUSiteFactBuilderOptions
        {
            PodSite = siteA,
            PouSite = pouSite
        });
        
        await PODSiteToPOUSiteFactBuilder.Load(db, new PODSiteToPOUSiteFactBuilderOptions
        {
            PodSite = podSite,
            PouSite = siteA
        });

        var rightA = await AllocationAmountsFactBuilder.Load(db);
        var rightB = await AllocationAmountsFactBuilder.Load(db);

        await SiteVariableAmountsFactBuilder.Load(db, new SiteVariableAmountsFactBuilderOptions
        {
            SiteDim = siteA
        });

        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = rightA
        });
        
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = rightB
        });

        var sourceA = await WaterSourcesDimBuilder.Load(db);
        var sourceB = await WaterSourcesDimBuilder.Load(db);
        
        await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            WaterSourcesDim = sourceA
        });
        
        await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            WaterSourcesDim = sourceB
        });
        
        var overlayA = await OverlayDimBuilder.Load(db);
        var overlayB = await OverlayDimBuilder.Load(db);
        
        await OverlayBridgeSitesFactBuilder.Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            RegulatoryOverlayDim = overlayA
        });
        
        await OverlayBridgeSitesFactBuilder.Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            RegulatoryOverlayDim = overlayB
        });
        
        var request = new SiteSearchRequest
        {
            Limit = 10
        };
        
        var response = await ExecuteHandler(request);
        
        response.Sites.Should().HaveCount(3);
    }
    
    [DataTestMethod]
    [DataRow(["SiteTypeA", "SiteTypeB"])]
    [DataRow(["sitetypea", "SITETYPEB"])]
    [DataRow([" SiteTypeA", "SiteTypeB "])]
    public async Task SiteAccessor_SiteTypeFilter_ReturnsCorrectSites(string[] searchSiteTypes)
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var siteTypeA = await SiteTypeBuilder.Load(db, new SiteTypeBuilderOptions
        {
            WaDEName = "SiteTypeA"
        });
        var siteTypeB = await SiteTypeBuilder.Load(db, new SiteTypeBuilderOptions
        {
            WaDEName = "SiteTypeB"
        });
        
        var siteA = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            SiteTypeCvNavigation = siteTypeA
        });
        var siteB = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            SiteTypeCvNavigation = siteTypeB
        });
        await SitesDimBuilder.Load(db);

        var request = new SiteSearchRequest
        {
            SiteTypes = [siteTypeA.WaDEName, siteTypeB.WaDEName],
            Limit = 10
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(2);
        response.Sites.Select(s => s.SiteTypeWaDEName).Should().BeEquivalentTo(siteTypeA.WaDEName, siteTypeB.WaDEName);
        response.Sites.Select(s => s.SiteUuid).Should().BeEquivalentTo(siteA.SiteUuid, siteB.SiteUuid);
    }
    
    [TestMethod]
    public async Task SiteAccessor_StatesFilter_ReturnsCorrectSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        await SitesDimBuilder.Load(db);

        var request = new SiteSearchRequest
        {
            States = [siteA.StateCv, siteB.StateCv],
            Limit = 10
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(2);
        response.Sites.Select(s => s.StateCv).Should().BeEquivalentTo(siteA.StateCv, siteB.StateCv);
        response.Sites.Select(s => s.SiteUuid).Should().BeEquivalentTo(siteA.SiteUuid, siteB.SiteUuid);
    }

    [TestMethod]
    public async Task SiteAccessor_WaterSourcesFilter_ReturnsCorrectSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var waterSourceTypeA = await WaterSourceTypeBuilder.Load(db);
        var waterSourceTypeB = await WaterSourceTypeBuilder.Load(db);
        var waterSourceTypeC = await WaterSourceTypeBuilder.Load(db);

        var waterSourceA = await WaterSourcesDimBuilder.Load(db, new WaterSourcesDimBuilderOptions
        {
            WaterSourceType = waterSourceTypeA
        });
        var waterSourceB = await WaterSourcesDimBuilder.Load(db, new WaterSourcesDimBuilderOptions
        {
            WaterSourceType = waterSourceTypeB
        });
        var waterSourceC = await WaterSourcesDimBuilder.Load(db, new WaterSourcesDimBuilderOptions
        {
            WaterSourceType = waterSourceTypeC
        });
        
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        var siteC = await SitesDimBuilder.Load(db);
        
        await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            WaterSourcesDim = waterSourceA
        });
        
        await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            WaterSourcesDim = waterSourceB
        });
        
        await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            WaterSourcesDim = waterSourceC
        });
        
        await WaterSourceBridgeSitesFactBuilder.Load(db, new WaterSourceBridgeSitesFactBuilderOptions
        {
            SitesDim = siteC,
            WaterSourcesDim = waterSourceC
        });
        
        var request = new SiteSearchRequest
        {
            WaterSourcesTypes = [waterSourceTypeA.WaDEName, waterSourceTypeB.WaDEName],
            Limit = 10
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(2);
        response.Sites.Select(s => s.SiteUuid).Should().BeEquivalentTo(siteA.SiteUuid, siteB.SiteUuid);
        response.Sites.First(s => s.SiteUuid == siteA.SiteUuid).WaterSources
            .Should()
            .BeEquivalentTo([waterSourceA.Map<WaterSourceSummary>()]);
        response.Sites.First(s => s.SiteUuid == siteB.SiteUuid).WaterSources.Should()
            .BeEquivalentTo([waterSourceB.Map<WaterSourceSummary>(), waterSourceC.Map<WaterSourceSummary>()]);
    }
    
    [TestMethod]
    public async Task SiteAccessor_SiteUuidsFilter_ReturnsCorrectSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        await SitesDimBuilder.Load(db);

        var request = new SiteSearchRequest
        {
            SiteUuids = [siteA.SiteUuid, siteB.SiteUuid],
            Limit = 10
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(2);
        response.Sites.Select(s => s.SiteUuid).Should().BeEquivalentTo(siteA.SiteUuid, siteB.SiteUuid);
    }

    [TestMethod]
    public async Task SiteAccessor_OverlayUuidsFilter_ReturnsCorrectSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        
        var overlayA = await OverlayDimBuilder.Load(db);
        var overlayB = await OverlayDimBuilder.Load(db);
        
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        
        await OverlayBridgeSitesFactBuilder.Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            RegulatoryOverlayDim = overlayA
        });
        
        await OverlayBridgeSitesFactBuilder.Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            RegulatoryOverlayDim = overlayB
        });
        
        var request = new SiteSearchRequest
        {
            OverlayUuids = [overlayA.OverlayUuid],
            Limit = 10
        };
        
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(1);
        response.Sites.Select(s => s.SiteUuid).Should().BeEquivalentTo(siteA.SiteUuid);
    }

    [TestMethod]
    public async Task SiteAccessor_AllocationUuidsFilter_ReturnsCorrectSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        
        var rightA = await AllocationAmountsFactBuilder.Load(db);
        var rightB = await AllocationAmountsFactBuilder.Load(db);
        
        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);
        
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteA,
            AllocationAmountsFact = rightA
        });
        
        await AllocationBridgeSitesFactBuilder.Load(db, new AllocationBridgeSitesFactBuilderOptions
        {
            SitesDim = siteB,
            AllocationAmountsFact = rightB
        });
        
        var request = new SiteSearchRequest
        {
            AllocationUuids = [rightA.AllocationUUID],
            Limit = 10
        };
        
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(1);
        response.Sites.Select(s => s.SiteUuid).Should().BeEquivalentTo(siteA.SiteUuid);
    }

    private async Task<SiteSearchResponse> ExecuteHandler(SiteSearchRequest request)
    {
        return await new SiteSearchHandler(Configuration.GetConfiguration()).Handle(request);
    }

    private static Polygon CreateUtahPolygon()
    {
        var wkt = new NetTopologySuite.IO.WKTReader();
        return (Polygon) wkt.Read(
            "POLYGON ((-114.0271 42.016652, -111.027832 42.000325, -111.049805 41.037931, -109.017334 41.013066, -109.050293 37.011326, -114.0271 36.985003, -114.0271 42.016652))");
    }
}