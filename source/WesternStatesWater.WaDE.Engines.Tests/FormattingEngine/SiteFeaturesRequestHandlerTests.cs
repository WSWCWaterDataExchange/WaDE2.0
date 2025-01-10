using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using Point = NetTopologySuite.Geometries.Point;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

[TestClass]
public class SiteFeaturesRequestHandlerTests
{
    private readonly ISiteAccessor _siteAccessorMock = Mock.Create<ISiteAccessor>(Behavior.Strict);
    
    [TestInitialize]
    public void TestInitialize()
    {
        Environment.SetEnvironmentVariable("ServerUrl", "http://localhost");
        Environment.SetEnvironmentVariable("ApiPath", "/api");
    }

    [TestMethod]
    public async Task Handler_CallsSearch_WithRequestParameters()
    {
        // Arrange
        var mockSiteSearchResponse = new SiteSearchResponse()
        {
            Sites = []
        };
        _siteAccessorMock.Arrange(mock =>
                mock.Search<SiteSearchRequest, SiteSearchResponse>(Arg.IsAny<SiteSearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);

        var request = new SiteFeaturesRequest
        {
            BoundingBox = [[10, 10, 20, 20]],
            Limit = 123,
            LastSiteUuid = "foo"
        };

        // Act 
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        Mock.Assert(
            () => _siteAccessorMock.Search<SiteSearchRequest, SiteSearchResponse>(
                Arg.Matches<SiteSearchRequest>(req =>
                    req.FilterBoundary.ToString() == "POLYGON ((10 20, 20 20, 20 10, 10 10, 10 20))" &&
                    req.Limit == 123 &&
                    req.LastSiteUuid == "foo")), Occurs.Once());
    }

    [TestMethod]
    public async Task Handler_RequestCrossesAntiMeridian_SearchRequestContainsMultiPolygon()
    {
        // Arrange
        var mockSiteSearchResponse = new SiteSearchResponse
        {
            Sites = []
        };
        _siteAccessorMock.Arrange(mock =>
                mock.Search<SiteSearchRequest, SiteSearchResponse>(Arg.IsAny<SiteSearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);

        var request = new SiteFeaturesRequest
        {
            BoundingBox = [[170, 50, -160, 60]],
        };

        // Act 
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        Mock.Assert(
            () => _siteAccessorMock.Search<SiteSearchRequest, SiteSearchResponse>(
                Arg.Matches<SiteSearchRequest>(req =>
                    req.FilterBoundary.ToString() ==
                    "MULTIPOLYGON (((170 60, 180 60, 180 50, 170 50, 170 60)), ((-180 60, -160 60, -160 50, -180 50, -180 60)))")),
            Occurs.Once());
    }

    [TestMethod]
    public async Task Handler_Response_SetsFeatureAttributes()
    {
        // Arrange
        var mockSiteSearchResponse = new SiteSearchResponse
        {
            Sites =
            [
                new SiteSearchItem
                {
                    SiteUuid = "site-uuid",
                    SiteNativeId = "site-native-id",
                    SiteName = "site-name",
                    UsgsSiteId = "usgs-site-id",
                    SiteType = "site-type",
                    CoordinateMethod = "coordinate-method",
                    CoordinateAccuracy = "coordinate-accuracy",
                    GnisCode = "gnis-code",
                    EpsgCode = "epsg-code",
                    NhdNetworkStatus = "nhd-network-status",
                    NhdProduct = "nhd-product",
                    State = "state",
                    Huc8 = "huc8",
                    Huc12 = "huc12",
                    County = "county",
                    RightUuids = ["right-uuids"],
                    IsTimeSeries = true,
                    PodOrPouSite = "pod-or-pou-site",
                    WaterSources = ["water-sources"],
                    Overlays = ["overlays"],
                    Location = Point.Empty
                }
            ]
        };

        _siteAccessorMock.Arrange(mock =>
                mock.Search<SiteSearchRequest, SiteSearchResponse>(Arg.IsAny<SiteSearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);

        var request = new SiteFeaturesRequest();

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Features.Should().HaveCount(1);
        response.Features[0].Geometry.Should().Be(Point.Empty);
        response.Features[0].Attributes.Should().NotBeNull();
        response.Features[0].Attributes["id"].Should().Be("site-uuid");
        response.Features[0].Attributes["nId"].Should().Be("site-native-id");
        response.Features[0].Attributes["name"].Should().Be("site-name");
        response.Features[0].Attributes["usgsSiteId"].Should().Be("usgs-site-id");
        response.Features[0].Attributes["sType"].Should().Be("site-type");
        response.Features[0].Attributes["coordMethod"].Should().Be("coordinate-method");
        response.Features[0].Attributes["coordAcc"].Should().Be("coordinate-accuracy");
        response.Features[0].Attributes["gnisCode"].Should().Be("gnis-code");
        response.Features[0].Attributes["epsgCode"].Should().Be("epsg-code");
        response.Features[0].Attributes["nhdNetStat"].Should().Be("nhd-network-status");
        response.Features[0].Attributes["nhdProd"].Should().Be("nhd-product");
        response.Features[0].Attributes["state"].Should().Be("state");
        response.Features[0].Attributes["huc8"].Should().Be("huc8");
        response.Features[0].Attributes["huc12"].Should().Be("huc12");
        response.Features[0].Attributes["county"].Should().Be("county");
        response.Features[0].Attributes["rightUuids"].Should().BeEquivalentTo(new[] { "right-uuids" });
        response.Features[0].Attributes["isTimeSeries"].Should().Be(true);
        response.Features[0].Attributes["podOrPouSite"].Should().Be("pod-or-pou-site");
        response.Features[0].Attributes["waterSources"].Should().BeEquivalentTo(new[] { "water-sources" });
        response.Features[0].Attributes["overlays"].Should().BeEquivalentTo(new[] { "overlays" });
    }

    [TestMethod]
    public async Task Handler_Response_SetsLinks()
    {
        // Arrange
        var mockSiteSearchResponse = new SiteSearchResponse
        {
            Sites =
            [
                new SiteSearchItem
                {
                    SiteUuid = "AK_1",
                    Location = Point.Empty
                },
                new SiteSearchItem
                {
                    SiteUuid = "UT_1",
                    Location = Point.Empty
                }
            ],
            LastUuid = "UT_1"
        };
        
        _siteAccessorMock.Arrange(mock =>
                mock.Search<SiteSearchRequest, SiteSearchResponse>(Arg.IsAny<SiteSearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);
        
        var request = new SiteFeaturesRequest();
        
        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);
        
        // Assert
        response.Links.Should().NotBeNull();
        response.Links.Should().HaveCount(3);
        response.Links.Should().BeEquivalentTo([
            new Link
            {
                Href = "http://localhost/swagger/ui",
                Rel = "root",
                Title = "Landing page",
                Type = "text/html"
            },
            new Link
            {
                Href = "http://localhost/swagger.json",
                Rel = "root",
                Title = "Landing page",
                Type = "application/json"
            },
            new Link
            {
                Href = "http://localhost/api/collections/sites/items?next=UT_1",
                Rel = "next",
                Title = null,
                Type = "application/geo+json"
            }
        ]);
    }

    private SiteFeaturesRequestHandler CreateHandler()
    {
        return new SiteFeaturesRequestHandler(
            Configuration.GetConfiguration(),
            _siteAccessorMock);
    }
}