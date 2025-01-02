using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers;

[TestClass]
public class FeatureSearchHandlerTests
{
    private readonly IFormattingEngine _formattingEngineMock = Mock.Create<IFormattingEngine>(Behavior.Strict);

    [TestMethod]
    public async Task Handler_MapsSiteFeature_ShouldMap()
    {
        var mockResponse = new SiteFeaturesResponse
        {
            Features = [
            new Feature
            {
                Geometry = Polygon.Empty,
                Attributes = new AttributesTable
                {
                    {"id", "foo"},
                    {"hello", "world"}
                }
            }]
        };

        _formattingEngineMock.Arrange(mock =>
                mock.Format<FeaturesRequestBase, FeaturesResponseBase>(
                    Arg.IsAny<SiteFeaturesRequest>()))
            .ReturnsAsync(mockResponse);
        
        // Act
        var handler = CreateHandler();
        var result = await handler.Handle(new SiteFeaturesSearchRequest());
        
        // Assert
        result.Features[0].Attributes["id"].Should().Be("foo");
    }

    private FeaturesSearchHandler CreateHandler()
    {
        return new FeaturesSearchHandler(_formattingEngineMock);
    }
}