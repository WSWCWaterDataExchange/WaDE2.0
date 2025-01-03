using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Features;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Api.Handlers.V2;
using WesternStatesWater.WaDE.Tests.Helpers;
using Polygon = NetTopologySuite.Geometries.Polygon;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers.V2;

[TestClass]
public class SiteFeaturesSearchRequestHandlerTests
{
    private readonly IFormattingEngine _formattingEngineMock = Mock.Create<IFormattingEngine>(Behavior.Strict);

    [TestMethod]
    public async Task Handler_MapsRequest()
    {
        // Arrange
        var request = new SiteFeaturesSearchRequest
        {
            Limit = "10",
            Bbox = "1,2,3,4",
            Next = "UT_123"
        };

        var mockFormatResponse = new SiteFeaturesResponse();

        _formattingEngineMock.Arrange(mock =>
                mock.Format<SiteFeaturesRequest, SiteFeaturesResponse>(
                    Arg.IsAny<SiteFeaturesRequest>()))
            .ReturnsAsync(mockFormatResponse);

        // Act
        var handler = CreateHandler();
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        Mock.Assert(() =>
            _formattingEngineMock.Format<SiteFeaturesRequest, SiteFeaturesResponse>(
                Arg.Matches<SiteFeaturesRequest>(req =>
                    req.Limit == 10 &&
                    req.BoundingBox[0][0].Equals(1) &&
                    req.BoundingBox[0][1].Equals(2) &&
                    req.BoundingBox[0][2].Equals(3) &&
                    req.BoundingBox[0][3].Equals(4) &&
                    req.LastSiteUuid == "UT_123")), Occurs.Once());
    }

    [TestMethod]
    public async Task Handler_SetsDefaultLimit_ReturnsEmptyResults()
    {
        // Arrange
        var request = new SiteFeaturesSearchRequest
        {
            Limit = null
        };

        var mockFormatResponse = new SiteFeaturesResponse
        {
            Type = "FeatureCollection",
            Features = [],
            Links = []
        };

        _formattingEngineMock.Arrange(mock =>
                mock.Format<SiteFeaturesRequest, SiteFeaturesResponse>(Arg.IsAny<SiteFeaturesRequest>()))
            .ReturnsAsync(mockFormatResponse);

        // Act
        var handler = CreateHandler();
        var result = await handler.Handle(request);

        // Assert
        Mock.Assert(
            () => _formattingEngineMock.Format<SiteFeaturesRequest, SiteFeaturesResponse>(
                Arg.IsAny<SiteFeaturesRequest>()), Occurs.Once());
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Handler_CallsFormatSiteFeaturesRequest_ReturnsResults()
    {
        // Arrange
        var request = new SiteFeaturesSearchRequest
        {
            Limit = null
        };

        var mockFormatResponse = new SiteFeaturesResponse
        {
            Type = "FeatureCollection",
            Features =
            [
                new Feature(Polygon.Empty, new AttributesTable
                {
                    { "id", "foo" }
                })
            ],
            Links =
            [
                new Link
                {
                    Href = "https://localhost/",
                    Rel = "self",
                    Title = "Document",
                    Type = "application/geo+json"
                }
            ]
        };

        _formattingEngineMock.Arrange(mock =>
                mock.Format<SiteFeaturesRequest, SiteFeaturesResponse>(
                    Arg.IsAny<SiteFeaturesRequest>()))
            .ReturnsAsync(mockFormatResponse);

        // Act
        var handler = CreateHandler();
        var result = await handler.Handle(request);

        // Assert
        result.Features.Should().HaveCount(1);
        result.Features[0].Geometry.Should().Be(Polygon.Empty);
        result.Features[0].Attributes["id"].Should().NotBeNull();
        result.Links.Should().HaveCount(1);
    }

    private SiteFeaturesSearchRequestHandler CreateHandler()
    {
        return new SiteFeaturesSearchRequestHandler(_formattingEngineMock);
    }
}