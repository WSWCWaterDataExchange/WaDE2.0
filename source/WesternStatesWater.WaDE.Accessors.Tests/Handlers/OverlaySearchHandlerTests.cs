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
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Handlers;

[TestClass]
public class OverlaySearchHandlerTests : DbTestBase
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
            await RegulatoryOverlayDimBuilder.Load(db);
        }

        var request = new OverlaySearchRequest
        {
            Limit = limit
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(5);
        response.LastUuid.Should().BeNull();
    }
    
    [TestMethod]
    public async Task Handler_LimitSet_ReturnsCorrectAmount()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        List<OverlayDim> dbOverlays = new();
        for (var i = 0; i < 5; i++)
        {
            dbOverlays.Add(await RegulatoryOverlayDimBuilder.Load(db));
        }

        var request = new OverlaySearchRequest
        {
            Limit = 3
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(3);
        response.LastUuid.Should()
            .Be(dbOverlays.OrderBy(o => o.OverlayUuid).Select(o => o.OverlayUuid)
                .ElementAt(2));
    }

    [TestMethod]
    public async Task Handler_LastKeySet_PagesNextSet()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        List<OverlayDim> overlays = new();
        for (var i = 0; i < 10; i++)
        {
            overlays.Add(await RegulatoryOverlayDimBuilder.Load(db));
        }

        overlays.Sort((x, y) =>
            string.Compare(x.OverlayUuid, y.OverlayUuid, StringComparison.Ordinal));

        var request = new OverlaySearchRequest
        {
            Limit = 3,
            LastKey = overlays[2].OverlayUuid
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(3);
        response.Overlays.Select(a => a.OverlayUuid).Should().BeEquivalentTo(
            overlays.Skip(3).Take(3).Select(a => a.OverlayUuid));
    }

    [TestMethod]
    public async Task Handler_FilterOverlayUuid_ReturnsRequestedOverlays()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());
        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayB = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayC = await RegulatoryOverlayDimBuilder.Load(db);

        var request = new OverlaySearchRequest
        {
            OverlayUuids = [overlayA.OverlayUuid, overlayB.OverlayUuid],
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(2);
        response.Overlays.Select(a => a.OverlayUuid).Should().BeEquivalentTo(
            overlayA.OverlayUuid, overlayB.OverlayUuid);
    }

    [TestMethod]
    public async Task Handler_FilterSiteUuids_ReturnsRelatedOverlays()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var siteA = await SitesDimBuilder.Load(db);
        var siteB = await SitesDimBuilder.Load(db);

        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayB = await RegulatoryOverlayDimBuilder.Load(db);

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
        response.Overlays.Select(a => a.OverlayUuid).Should().BeEquivalentTo(
            overlayA.OverlayUuid, overlayB.OverlayUuid);
    }

    [TestMethod]
    public async Task Handler_FilterArea_ReturnsOverlaysInIntersectingArea()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var areaOutsideRequest = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = CreateVegasGeometry()
        });

        var areaInersectingRequest = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = CreateBearLakeGeometry()
        });

        var areaInsideRequest = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = CreateGreatSaltLakeGeometry()
        });

        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayB = await RegulatoryOverlayDimBuilder.Load(db);
        var omittedOverlay = await RegulatoryOverlayDimBuilder.Load(db);

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaInersectingRequest
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayB,
            ReportingUnits = areaInsideRequest
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = omittedOverlay,
            ReportingUnits = areaOutsideRequest
        });

        var request = new OverlaySearchRequest
        {
            GeometrySearch = new SpatialSearchCriteria
            {
                Geometry = CreateUtahPolygon(),
                SpatialRelationType = SpatialRelationType.Intersects
            },
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(2);
        response.Overlays.Select(a => a.OverlayUuid).Should().BeEquivalentTo(
            overlayA.OverlayUuid, overlayB.OverlayUuid);
    }

    [TestMethod]
    public async Task Handler_OverlaysWithIntersectingAreas_CombinedIntoASingleGeometryPolygon()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var wkt = new NetTopologySuite.IO.WKTReader();

        // Create Intersecting Polygons
        var polygonA = (Polygon) wkt.Read(
            "POLYGON ((-105.394592 39.7093, -105.174866 39.7093, -105.174866 39.907629, -105.394592 39.907629, -105.394592 39.7093))");
        var polygonB =
            (Polygon) wkt.Read(
                "POLYGON ((-105.227051 39.7093, -104.963379 39.7093, -104.963379 39.907629, -105.227051 39.907629, -105.227051 39.7093))");
        var polygonC =
            (Polygon) wkt.Read(
                "POLYGON ((-105.312195 39.849667, -105.099335 39.849667, -105.099335 40.004476, -105.312195 40.004476, -105.312195 39.849667))");

        var areaA = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonA
        });

        var areaB = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonB
        });

        var areaC = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonC
        });

        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaA
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaB
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaC
        });

        var request = new OverlaySearchRequest
        {
            Limit = 5
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(1);
        response.Overlays[0].Areas.Should().BeOfType<Polygon>();
    }

    [TestMethod]
    public async Task Handler_OverlaysWithNonIntersectingAreas_CombinedIntoASingleGeometryMultiPolygon()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var wkt = new NetTopologySuite.IO.WKTReader();

        // Create Non-Intersecting Polygons
        var polygonA = (Polygon) wkt.Read(
            "POLYGON ((-106.929932 43.452919, -106.358643 43.452919, -106.358643 43.771094, -106.929932 43.771094, -106.929932 43.452919))");
        var polygonB =
            (Polygon) wkt.Read(
                "POLYGON ((-106.11969 43.548548, -105.970001 43.548548, -105.970001 43.674825, -106.11969 43.674825, -106.11969 43.548548))");
        var polygonC =
            (Polygon) wkt.Read(
                "POLYGON ((-107.381744 43.569447, -107.124939 43.569447, -107.124939 43.658931, -107.381744 43.658931, -107.381744 43.569447))");

        var areaA = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonA
        });

        var areaB = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonB
        });

        var areaC = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonC
        });

        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);


        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaA
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaB
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaC
        });

        var request = new OverlaySearchRequest
        {
            Limit = 5
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(1);
        response.Overlays[0].Areas.Should().BeOfType<MultiPolygon>();
    }

    [TestMethod]
    public async Task Handler_OverlayWithManyAreas_ReturnsRelatedAreaInformation()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var wkt = new NetTopologySuite.IO.WKTReader();

        var polygonA = (Polygon) wkt.Read(
            "POLYGON ((-106.929932 43.452919, -106.358643 43.452919, -106.358643 43.771094, -106.929932 43.771094, -106.929932 43.452919))");
        var polygonB =
            (Polygon) wkt.Read(
                "POLYGON ((-106.11969 43.548548, -105.970001 43.548548, -105.970001 43.674825, -106.11969 43.674825, -106.11969 43.548548))");
        var polygonC =
            (Polygon) wkt.Read(
                "POLYGON ((-107.381744 43.569447, -107.124939 43.569447, -107.124939 43.658931, -107.381744 43.658931, -107.381744 43.569447))");

        var areaA = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonA
        });

        var areaB = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonB
        });

        var areaC = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            Geometry = polygonC
        });

        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);


        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaA
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaB
        });

        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaC
        });

        var request = new OverlaySearchRequest
        {
            Limit = 5
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(1);
        response.Overlays[0].ReportingAreas.Select(area => area.ReportingUnitName).Should().BeEquivalentTo(
            areaA.ReportingUnitName, areaB.ReportingUnitName, areaC.ReportingUnitName);
        response.Overlays[0].ReportingAreas.Select(area => area.ReportingUnitNativeId).Should().BeEquivalentTo(
            areaA.ReportingUnitNativeId, areaB.ReportingUnitNativeId, areaC.ReportingUnitNativeId);
    }
    
    [TestMethod]
    public async Task Handler_FilterStates_ReturnsOverlaysInRequestedStates()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var stateA = await StateBuilder.Load(db);
        var stateB = await StateBuilder.Load(db);

        var areaA = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            State = stateA
            
        });
        var areaB = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            State = stateB
        });
        var areaC = await ReportingUnitsDimBuilder.Load(db, new ReportingUnitsDimBuilderOptions
        {
            State = stateA
        });

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
        await RegulatoryReportingUnitsFactBuilder.Load(db, new RegulatoryReportingUnitsFactBuilderOptions
        {
            RegulatoryOverlay = overlayA,
            ReportingUnits = areaC
        });
        
        var request = new OverlaySearchRequest
        {
            States = [stateA.Name],
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(1);
        response.Overlays.Select(a => a.OverlayUuid).Should().BeEquivalentTo(overlayA.OverlayUuid);
        response.Overlays[0].ReportingAreas[0].State.Should().BeEquivalentTo(stateA.Name);
    }
    
    [TestMethod]
    public async Task Handler_FilterOverlayTypes_ReturnsOverlaysWithRequestedTypes()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayB = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayC = await RegulatoryOverlayDimBuilder.Load(db);

        string[] uniqueOverlayTypes =
            [overlayA.OverlayType.WaDEName, overlayB.OverlayType.WaDEName, overlayC.OverlayType.WaDEName];
        uniqueOverlayTypes.Should().OnlyHaveUniqueItems();

        var request = new OverlaySearchRequest
        {
            OverlayTypes = [overlayA.OverlayType.WaDEName, overlayB.OverlayType.WaDEName],
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(2);
        response.Overlays.Select(a => a.OverlayUuid).Should().BeEquivalentTo(
            overlayA.OverlayUuid, overlayB.OverlayUuid);
    }
    
    [TestMethod]
    public async Task Handler_FilterWaterSourceTypes_ReturnsOverlaysWithRequestedTypes()
    {
        // Arrange
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var overlayA = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayB = await RegulatoryOverlayDimBuilder.Load(db);
        var overlayC = await RegulatoryOverlayDimBuilder.Load(db);

        string[] uniqueWaterSourceTypes =
            [overlayA.WaterSourceType.WaDEName, overlayB.WaterSourceType.WaDEName, overlayC.WaterSourceType.WaDEName];
        uniqueWaterSourceTypes.Should().OnlyHaveUniqueItems();

        var request = new OverlaySearchRequest
        {
            WaterSourceTypes = [overlayA.WaterSourceType.WaDEName, overlayB.WaterSourceType.WaDEName],
            Limit = 10
        };

        // Act
        var response = await ExecuteHandler(request);

        // Assert
        response.Overlays.Should().HaveCount(2);
        response.Overlays.Select(a => a.OverlayUuid).Should().BeEquivalentTo(
            overlayA.OverlayUuid, overlayB.OverlayUuid);
    }

    private async Task<OverlaySearchResponse> ExecuteHandler(OverlaySearchRequest request)
    {
        var handler = new OverlaySearchHandler(Configuration.GetConfiguration());
        return await handler.Handle(request);
    }

    /// <summary>
    /// https://wktmap.com/?c6f14b22
    /// </summary>
    /// <returns>Polygon of Utah state.</returns>
    private static Polygon CreateUtahPolygon()
    {
        var wkt = new NetTopologySuite.IO.WKTReader();
        return (Polygon) wkt.Read(
            "POLYGON ((-114.0271 42.016652, -111.027832 42.000325, -111.049805 41.037931, -109.017334 41.013066, -109.050293 37.011326, -114.0271 36.985003, -114.0271 42.016652))");
    }

    /// <summary>
    /// https://wktmap.com/?2abc3c2b
    /// </summary>
    /// <returns>Polygon around Great Salt Lake in Utah.</returns>
    private static Geometry CreateGreatSaltLakeGeometry()
    {
        var wkt = new NetTopologySuite.IO.WKTReader();
        return wkt.Read(
            "POLYGON ((-113.049316 40.605612, -113.049316 41.73033, -111.741943 41.73033, -111.741943 40.605612, -113.049316 40.605612))");
    }

    /// <summary>
    /// https://wktmap.com/?96542a0c
    /// </summary>
    /// <returns>Polygon around Bear Lake that is in Idaho and Utah.</returns>
    private static Geometry CreateBearLakeGeometry()
    {
        var wkt = new NetTopologySuite.IO.WKTReader();
        return wkt.Read(
            "POLYGON ((-111.43158 41.834781, -111.43158 42.171546, -111.222839 42.171546, -111.222839 41.834781, -111.43158 41.834781))");
    }

    /// <summary>
    /// https://wktmap.com/?31a14eec
    /// </summary>
    /// <returns>Polygon around Last Vegas, NV.</returns>
    private static Geometry CreateVegasGeometry()
    {
        var wkt = new NetTopologySuite.IO.WKTReader();
        return wkt.Read(
            "POLYGON ((-115.375671 35.920196, -115.375671 36.339466, -114.906006 36.339466, -114.906006 35.920196, -115.375671 35.920196))");
    }
}