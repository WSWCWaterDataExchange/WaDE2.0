using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers.V2;

[TestClass]
public class SiteFeaturesSearchRequestHandlerTests
{
    private readonly IFormattingEngine _formattingEngineMock = Mock.Create<IFormattingEngine>(Behavior.Strict);
    private readonly ISiteAccessor _siteAccessorMock = Mock.Create<ISiteAccessor>(Behavior.Strict);

    [TestMethod]
    public async Task Handler_CallsSearchThenFormatsSearch_ReturnsResponse()
    {
        // Arrange
        var request = new SiteFeaturesSearchRequest();
        var mockSearchResponse = new SiteSearchResponse
        {
            Sites = []
        };
        
        _siteAccessorMock.Arrange(mock =>
                mock.Search<SiteSearchRequest, SiteSearchResponse>(Arg.IsAny<SiteSearchRequest>()))
            .ReturnsAsync(mockSearchResponse);

        var mockFormatResponse = new OgcFeaturesFormattingResponse();
        _formattingEngineMock.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(mockFormatResponse);

        // Act
        var handler = CreateHandler();
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        Mock.Assert(() =>
            _siteAccessorMock.Search<SiteSearchRequest, SiteSearchResponse>(
                Arg.IsAny<SiteSearchRequest>()), Occurs.Once());
    }
    
    [DataTestMethod]
    [DataRow(null)]
    [DataRow("123456789")]
    public async Task Handler_SetsDefaultLimit_ReturnsEmptyResults(string limit)
    {
        // Arrange
        var request = new SiteFeaturesSearchRequest
        {
            Limit = limit
        };
        
        var mockSearchResponse = new SiteSearchResponse
        {
            Sites = []
        };
        _siteAccessorMock.Arrange(mock =>
                mock.Search<SiteSearchRequest, SiteSearchResponse>(Arg.IsAny<SiteSearchRequest>()))
            .ReturnsAsync(mockSearchResponse);

        var mockFormatResponse = new OgcFeaturesFormattingResponse
        {
            Features = [],
            Links = []
        };
        _formattingEngineMock.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(mockFormatResponse);

        // Act
        var handler = CreateHandler();
        var result = await handler.Handle(request);

        // Assert
        Mock.Assert(
            () => _siteAccessorMock.Search<SiteSearchRequest, SiteSearchResponse>(
                Arg.Matches<SiteSearchRequest>(req =>
                    req.Limit == 1_000)), Occurs.Once());
        Assert.IsNotNull(result);
    }

    private SiteFeaturesSearchRequestHandler CreateHandler()
    {
        return new SiteFeaturesSearchRequestHandler(_formattingEngineMock, _siteAccessorMock);
    }
}