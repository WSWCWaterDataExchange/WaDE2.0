using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

[TestClass]
public class OverlayFeaturesRequestHandlerTests
{
    private readonly IRegulatoryOverlayAccessor _overlayAccessor =
        Mock.Create<IRegulatoryOverlayAccessor>(Behavior.Strict);

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
        var mockSiteSearchResponse = new OverlaySearchResponse
        {
            Overlays = []
        };
        _overlayAccessor.Arrange(mock =>
                mock.Search<OverlaySearchRequest, OverlaySearchResponse>(Arg.IsAny<OverlaySearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);

        var request = new OverlayFeaturesRequest
        {
            Bbox = [[10, 10, 20, 20]],
            Limit = 123,
            LastOverlayUuid = "foo"
        };

        // Act 
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        Mock.Assert(
            () => _overlayAccessor.Search<OverlaySearchRequest, OverlaySearchResponse>(
                Arg.Matches<OverlaySearchRequest>(req =>
                    req.FilterBoundary.ToString() == "POLYGON ((10 20, 20 20, 20 10, 10 10, 10 20))" &&
                    req.Limit == 123 &&
                    req.LastKey == "foo")), Occurs.Once());
    }

    [TestMethod]
    public async Task Handler_RequestCrossesAntiMeridian_SearchRequestContainsMultiPolygon()
    {
        // Arrange
        var mockSiteSearchResponse = new OverlaySearchResponse
        {
            Overlays = []
        };
        _overlayAccessor.Arrange(mock =>
                mock.Search<OverlaySearchRequest, OverlaySearchResponse>(Arg.IsAny<OverlaySearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);

        var request = new OverlayFeaturesRequest
        {
            Bbox = [[170, 50, -160, 60]],
        };

        // Act 
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        Mock.Assert(
            () => _overlayAccessor.Search<OverlaySearchRequest, OverlaySearchResponse>(
                Arg.Matches<OverlaySearchRequest>(req =>
                    req.FilterBoundary.ToString() ==
                    "MULTIPOLYGON (((170 60, 180 60, 180 50, 170 50, 170 60)), ((-180 60, -160 60, -160 50, -180 50, -180 60)))")),
            Occurs.Once());
    }

    [TestMethod]
    public async Task Handler_Response_SetsFeatureAttributes()
    {
        // Arrange
        var mockSiteSearchResponse = new OverlaySearchResponse
        {
            Overlays =
            [
                new OverlaySearchItem
                {
                    Areas = Point.Empty,
                    AreaNames = ["area1", "area2"],
                    AreaNativeIds = ["1", "2"],
                    OverlayNativeId = "overlay-native-id",
                    OverlayType = "overlay-type",
                    OverlayUuid = "overlay-uuid",
                    OversightAgency = "oversight-agency",
                    RegulatoryDescription = "regulatory-description",
                    RegulatoryName = "regulatory-name",
                    RegulatoryStatute = "regulatory-statute",
                    RegulatoryStatuteLink = "regulatory-statute-link",
                    RegulatoryStatus = "regulatory-status",
                    StatutoryEffectiveDate = "statutory-effective-date",
                    StatutoryEndDate = "statutory-end-date",
                    WaterSource = "water-source",
                    SiteUuids = ["site-uuids"]
                }
            ]
        };

        _overlayAccessor.Arrange(mock =>
                mock.Search<OverlaySearchRequest, OverlaySearchResponse>(Arg.IsAny<OverlaySearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);

        var request = new OverlayFeaturesRequest();

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Features.Should().HaveCount(1);
        response.Features[0].Geometry.Should().Be(Point.Empty);
        response.Features[0].Attributes.Should().NotBeNull();
        response.Features[0].Attributes["id"].Should().Be("overlay-uuid");
        response.Features[0].Attributes["nId"].Should().Be("overlay-native-id");
        response.Features[0].Attributes["name"].Should().Be("regulatory-name");
        response.Features[0].Attributes["desc"].Should().Be("regulatory-description");
        response.Features[0].Attributes["status"].Should().Be("regulatory-status");
        response.Features[0].Attributes["agency"].Should().Be("oversight-agency");
        response.Features[0].Attributes["statute"].Should().Be("regulatory-statute");
        response.Features[0].Attributes["statueRef"].Should().Be("regulatory-statute-link");
        response.Features[0].Attributes["statueEffDate"].Should().Be("statutory-effective-date");
        response.Features[0].Attributes["statueEndDate"].Should().Be("statutory-end-date");
        response.Features[0].Attributes["oType"].Should().Be("overlay-type");
        response.Features[0].Attributes["wSource"].Should().Be("water-source");
        response.Features[0].Attributes["areaNames"].Should().BeEquivalentTo(new[] { "area1", "area2" });
        response.Features[0].Attributes["areaNIds"].Should().BeEquivalentTo(new[] { "1", "2" });
        response.Features[0].Attributes["sites"].Should().BeEquivalentTo(new[] { "site-uuids" });
    }

    [TestMethod]
    public async Task Handler_Response_SetsLinks()
    {
        // Arrange
        var mockSiteSearchResponse = new OverlaySearchResponse()
        {
            Overlays =
            [
                new OverlaySearchItem
                {
                    OverlayUuid = "AK_1",
                    Areas = Polygon.Empty
                },
                new OverlaySearchItem
                {
                    OverlayUuid = "UT_1",
                    Areas = Polygon.Empty
                }
            ]
        };

        _overlayAccessor.Arrange(mock =>
                mock.Search<OverlaySearchRequest, OverlaySearchResponse>(Arg.IsAny<OverlaySearchRequest>()))
            .ReturnsAsync(mockSiteSearchResponse);

        var request = new OverlayFeaturesRequest();

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Links.Should().NotBeNull();
        response.Links.Should().HaveCount(3);
        response.Links.Select(link => link.Rel).Should().Contain("next");
    }

    private OverlayFeaturesRequestHandler CreateHandler()
    {
        return new OverlayFeaturesRequestHandler(
            Configuration.GetConfiguration(),
            _overlayAccessor
        );
    }
}