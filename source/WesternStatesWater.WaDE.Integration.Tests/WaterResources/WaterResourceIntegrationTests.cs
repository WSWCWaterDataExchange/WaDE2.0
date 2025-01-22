using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.IO;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Integration.Tests.WaterResources;

[TestClass]
public class WaterResourceIntegrationTests : IntegrationTestsBase
{
    private IWaterResourceManager _manager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _manager = Services.GetRequiredService<IWaterResourceManager>();
    }
    

    [DataTestMethod]
    [DataRow(1, 0, 1, 1, DisplayName = "Should return the total count requested (1)")]
    [DataRow(3, 0, 3, 3, DisplayName = "Should return the total count requested (3)")]
    [DataRow(2, 0, 1, 1, DisplayName = "Should return the max records if less than requested")]
    [DataRow(4, 3, 10, 1, DisplayName = "Should start at the requested index")]
    [DataRow(5, 6, 1, 0, DisplayName = "Should return no records if start index is greater than total count")]
    public async Task Load_AggregatedAmountsSearchRequest_PagingSmokeTests(
        int saveCount,
        int requestStartIndex,
        int requestCount,
        int expectedResultCount
    )
    {
        var facts = await SaveAggregatedAmountsFacts(saveCount);

        var request = new AggregatedAmountsSearchRequest
        {
            Filters = new AggregatedAmountsFilters(),
            StartIndex = requestStartIndex,
            RecordCount = requestCount
        };

        var response = await _manager.Load<AggregatedAmountsSearchRequest, AggregatedAmountsSearchResponse>(request);

        // The total is always the total number of facts saved.
        response.AggregatedAmounts.TotalAggregatedAmountsCount.Should().Be(saveCount);

        // The number of organizations returned should match the record count requested.
        response.AggregatedAmounts.Organizations.Should().HaveCount(expectedResultCount);

        // The aggregated amounts should be returned in the order they were saved (i.e., ordered on ID).
        var expectedLastOrgName = facts
            .ElementAtOrDefault(requestStartIndex + expectedResultCount - 1)?
            .Organization
            .OrganizationName;

        response.AggregatedAmounts
            .Organizations
            .LastOrDefault()?
            .OrganizationName
            .Should()
            .Be(expectedLastOrgName);
    }

    private async Task<EF.AggregatedAmountsFact[]> SaveAggregatedAmountsFacts(int count)
    {
        await using var db = new EF.WaDEContext(Services.GetRequiredService<IConfiguration>());

        var facts = new List<EF.AggregatedAmountsFact>(count);

        for (var i = 0; i < count; i++)
        {
            var fact = await AggregatedAmountsFactBuilder.Load(db);
            facts.Add(fact);
        }

        return facts.ToArray();
    }

    [TestMethod]
    public async Task Load_SiteFeaturesSearchRequests_ShouldNotReturnSitesWithInvalidGeometriesIfSearchingInBbox()
    {
        MockRequestPath("/collections/sites/items");
        
        await using var db = new EF.WaDEContext(Services.GetRequiredService<IConfiguration>());

        // Closed polygon over Lincoln, NE, with intersecting lines.
        var invalidPolygon = GeometryExtensions.GetGeometryByWkt(
            "POLYGON ((-96.67735074089035 40.769796285659815, -96.75676926449917 40.754715616917274, -96.78923445807378 40.739875942484574, -96.66341614557061 40.85006621914999, -96.69796507752979 40.857495918824725, -96.67735074089035 40.769796285659815))");

        await SitesDimBuilder.Load(db, new SitesDimBuilderOptions { Geometry = invalidPolygon });

        var request = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest
        {
            Bbox = "-180, -90, 180, 90",
            Limit = "10"
        };

        var response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(request);

        // Filtered on geometry, can't find the invalid site.
        response.Features.Should().BeEmpty();

        request = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest
        {
            Limit = "10"
        };

        response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(request);

        // Not filtered on geometry, everything is returned.
        response.Features.Length.Should().Be(1);
    }

    [TestMethod]
    public async Task Load_SiteFeaturesSearchRequest_ShouldLoadSitesWithDifferentGeometryTypes()
    {
        MockRequestPath("/collections/sites/items");
        
        await using var db = new EF.WaDEContext(Services.GetRequiredService<IConfiguration>());

        var geographyFaker = new Faker().Geography();

        var siteOptions = new SitesDimBuilderOptions[]
        {
            new() { SitePoint = geographyFaker.RandomPoint(40.7, 40.9, -96.8, -96.6) },
            new() { SitePoint = geographyFaker.RandomMultiPoint(40.7, 40.9, -96.8, -96.6) },
            new() { Geometry = geographyFaker.RandomPolygon(40.7, 40.9, -96.8, -96.6) },
            new() { Geometry = geographyFaker.RandomMultiPolygon(40.7, 40.9, -96.8, -96.6) }
        };

        await SitesDimBuilder.Load(db, siteOptions);

        var request = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest
        {
            Bbox = "-96.8, 40.7, -96.6, 41",
            Limit = "4"
        };

        var response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(request);

        response.Features.Should().HaveCount(siteOptions.Length);
    }

    [TestMethod]
    public async Task Load_SiteFeaturesSearchRequest_ShouldSearchWithinABbox()
    {
        MockRequestPath("/collections/sites/items");
        await using var db = new EF.WaDEContext(Services.GetRequiredService<IConfiguration>());

        var geographyFaker = new Faker().Geography();

        var lincolnSiteOptions = Enumerable
            .Range(0, 5).Select(_ => new SitesDimBuilderOptions
            {
                SitePoint = geographyFaker.RandomPoint(40.7, 40.9, -96.8, -96.6)
            })
            .ToArray();

        var omahaSiteOptions = Enumerable
            .Range(0, 3).Select(_ => new SitesDimBuilderOptions
            {
                SitePoint = geographyFaker.RandomPoint(41.2, 41.4, -96.0, -95.8)
            })
            .ToArray();

        var lincolnSites = await SitesDimBuilder.Load(db, lincolnSiteOptions);
        var omahaSites = await SitesDimBuilder.Load(db, omahaSiteOptions);

        var lincolnSearchRequest = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest
        {
            Bbox = "-96.8, 40.7, -96.6, 41",
            Limit = "10"
        };

        var response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(lincolnSearchRequest);

        response.Features.Length
            .Should()
            .Be(lincolnSites.Length, "all these points should be in the Lincoln area.");

        response.Features
            .Select(f => f.Attributes["id"])
            .Should()
            .Contain(lincolnSites.Select(s => s.SiteUuid));

        var omahaSearchRequest = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest
        {
            Bbox = "-96.0, 41.2, -95.8, 41.4",
            Limit = "10"
        };

        response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(omahaSearchRequest);

        response.Features.Length
            .Should()
            .Be(omahaSites.Length, "all these points should be in the Omaha area.");

        response.Features
            .Select(f => f.Attributes["id"])
            .Should()
            .Contain(omahaSites.Select(s => s.SiteUuid));

        var easternNebraskaSearchRequest = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest
        {
            Bbox = "-97.5, 40.0, -95.5, 42.5",
            Limit = "10"
        };

        response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(easternNebraskaSearchRequest);

        response.Features.Length
            .Should()
            .Be(lincolnSites.Length + omahaSites.Length, "all these points should be in the eastern Nebraska box.");
    }

    [TestMethod]
    public async Task Load_SiteFeaturesItemRequest_ShouldReturnNextLinkUntilTheFinalPage()
    {
        MockRequestPath("/collections/sites/items?limit=1");

        await using var db = new EF.WaDEContext(Services.GetRequiredService<IConfiguration>());

        var geographyFaker = new Faker().Geography();

        var siteOptions = new SitesDimBuilderOptions[]
        {
            new() { SitePoint = geographyFaker.RandomPoint() },
            new() { SitePoint = geographyFaker.RandomPoint() },
            new() { SitePoint = geographyFaker.RandomPoint() },
            new() { SitePoint = geographyFaker.RandomPoint() }
        };

        var sites = (await SitesDimBuilder.Load(db, siteOptions))
            .OrderBy(site => site.SiteUuid)
            .ToArray();

        // Get the first page (of 1).
        var request = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest { Limit = "1" };

        var response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(request);

        sites[0].SiteUuid.Should().Be(response.Features[0].Attributes["id"].ToString());

        // Use the link to get the next page, but up the limit to 2.
        var next = response.Links.First(link => link.Rel == "next").Href.Split('=').Last();
        
        MockRequestPath("/collections/sites/items?limit=2&next=" + next);


        request = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest { Limit = "2", Next = next };

        response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(request);

        sites[1].SiteUuid.Should().Be(response.Features[0].Attributes["id"].ToString());
        sites[2].SiteUuid.Should().Be(response.Features[1].Attributes["id"].ToString());

        // Use the link to get the final page. Overshoot the limit for good measure.
        next = response.Links.First(link => link.Rel == "next").Href.Split('=').Last();
        
        MockRequestPath("/collections/sites/items?limit=10&next=" + next);

        request = new Contracts.Api.Requests.V2.SiteFeaturesItemRequest { Limit = "10", Next = next };

        response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(request);

        response.Features.Length.Should().Be(1);
        sites[3].SiteUuid.Should().Be(response.Features[0].Attributes["id"].ToString());
        response.Links.Count(link => link.Rel == "next").Should().Be(0);
    }
    
    [TestMethod]
    public async Task Load_SiteFeaturesAreaRequest_ShouldSearchWithinWktCoords()
    {
        MockRequestPath("/collections/sites/area");
        await using var db = new EF.WaDEContext(Services.GetRequiredService<IConfiguration>());

        var geographyFaker = new Faker().Geography();

        var lincolnSiteOptions = Enumerable
            .Range(0, 5).Select(_ => new SitesDimBuilderOptions
            {
                SitePoint = geographyFaker.RandomPoint(40.7, 40.9, -96.8, -96.6)
            })
            .ToArray();

        var omahaSiteOptions = Enumerable
            .Range(0, 3).Select(_ => new SitesDimBuilderOptions
            {
                SitePoint = geographyFaker.RandomPoint(41.2, 41.4, -96.0, -95.8)
            })
            .ToArray();
        
        var lincolnSites = await SitesDimBuilder.Load(db, lincolnSiteOptions);
        var omahaSites = await SitesDimBuilder.Load(db, omahaSiteOptions);

        var lincolnSearchRequest = new Contracts.Api.Requests.V2.SiteFeaturesAreaRequest
        {
            Coords = "POLYGON((-96.8 41, -96.8 40.7, -96.6 40.7, -96.6 41, -96.8 41))",
            Limit = "10"
        };

        var response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(lincolnSearchRequest);

        response.Features.Length
            .Should()
            .Be(lincolnSites.Length, "all these points should be in the Lincoln area.");

        response.Features
            .Select(f => f.Attributes["id"])
            .Should()
            .Contain(lincolnSites.Select(s => s.SiteUuid));

        var omahaSearchRequest = new Contracts.Api.Requests.V2.SiteFeaturesAreaRequest
        {
            Coords = "POLYGON((-96.0 41.4, -96.0 41.2, -95.8 41.2, -95.8 41.4, -96.0 41.4))",
            Limit = "10"
        };

        response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(omahaSearchRequest);

        var omahaGeometrySearch = new List<NetTopologySuite.Geometries.Geometry>();
        omahaGeometrySearch.AddRange(omahaSites.Select(x => x.SitePoint));
        var wkt = new WKTReader();
        omahaGeometrySearch.Add(wkt.Read("POLYGON((-96.0 41.4, -96.0 41.2, -95.86 41.2, -95.8 41.4, -96.0 41.4))"));
        var searchGeometry= NetTopologySuite.Geometries.Utilities.GeometryCombiner.Combine(omahaGeometrySearch);

        response.Features.Length
            .Should()
            .Be(omahaSites.Length, "all these points should be in the Omaha area.");

        response.Features
            .Select(f => f.Attributes["id"])
            .Should()
            .Contain(omahaSites.Select(s => s.SiteUuid));

        var easternNebraskaSearchRequest = new Contracts.Api.Requests.V2.SiteFeaturesAreaRequest
        {
            Coords = "POLYGON((-97.5 42.5, -97.5 40, -95.5 40.0, -95.5 42.5, -97.5 42.5))",
            Limit = "10"
        };

        response = await _manager.Search<
            Contracts.Api.Requests.SiteFeaturesSearchRequestBase,
            Contracts.Api.Responses.V2.SiteFeaturesSearchResponse
        >(easternNebraskaSearchRequest);

        response.Features.Length
            .Should()
            .Be(lincolnSites.Length + omahaSites.Length, "all these points should be in the eastern Nebraska area.");
    }
}