using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests.Handlers;

[TestClass]
public class SiteSearchHandlerTests : DbTestBase
{
    [TestMethod]
    public async Task SiteAccessor_GeometryFilter_ReturnsIntersectedSites()
    {
        await using var db = new WaDEContext(Configuration.GetConfiguration());

        var filterPolygon = CreateUtahPolygon();

        var wkt = new NetTopologySuite.IO.WKTReader();
        // Intersecting polygon
        var siteA = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = wkt.Read(
                "POLYGON ((-114.400635 41.574361, -114.400635 42.204107, -113.57666 42.204107, -113.57666 41.574361, -114.400635 41.574361))")
        });

        // Points inside boundary
        var siteB = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = CreateRandomPointInsidePolygon(filterPolygon)
        });
        var siteC = await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = CreateRandomPointInsidePolygon(filterPolygon)
        });

        // Point outside boundary
        await SitesDimBuilder.Load(db, new SitesDimBuilderOptions
        {
            Geometry = wkt.Read("POINT (-115.026855 40.988192)")
        });


        var request = new SiteSearchRequest
        {
            Filters = new SiteFilters
            {
                FilterBoundary = filterPolygon,
                Limit = 10
            }
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(3);
        response.Sites.Select(s => s.SiteUUID).Should()
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
            Filters = new SiteFilters
            {
                Limit = 3,
            }
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(3);
        response.Sites.Select(site => site.SiteUUID).Should().BeInAscendingOrder();
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

        sites.Sort((x, y) => String.Compare(x.SiteUuid, y.SiteUuid, StringComparison.Ordinal));

        var request = new SiteSearchRequest
        {
            Filters = new SiteFilters
            {
                LastSiteUuid = sites[3].SiteUuid,
                Limit = 3,
            }
        };
        var response = await ExecuteHandler(request);
        response.Sites.Should().HaveCount(3);
        response.Sites.Select(s => s.SiteUUID).Should().BeEquivalentTo(
            sites[4].SiteUuid,
            sites[5].SiteUuid,
            sites[6].SiteUuid
        );
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

    public static Point CreateRandomPointInsidePolygon(Polygon polygon)
    {
        var random = new Random();
        var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory();
        return gf.CreatePoint(
            new Coordinate(
                random.NextDouble() * (polygon.EnvelopeInternal.MaxX - polygon.EnvelopeInternal.MinX) +
                polygon.EnvelopeInternal.MinX,
                random.NextDouble() * (polygon.EnvelopeInternal.MaxY - polygon.EnvelopeInternal.MinY) +
                polygon.EnvelopeInternal.MinY
            )
        );
    }
}