using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.Shared.Errors;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers.V2;

[TestClass]
public class TimeSeriesFeatureItemGetRequestHandlerTests
{
    private ISiteVariableAmountsAccessor _siteVariableAmountsAccessorMock = Mock.Create<ISiteVariableAmountsAccessor>();
    private IFormattingEngine _formattingEngine = Mock.Create<IFormattingEngine>();

    [TestMethod]
    public async Task Handler_FoundOneSite_ReturnsResponse()
    {
        // Arrange
        string requestedSiteUuid = "1234";
        _siteVariableAmountsAccessorMock.Arrange(mock =>
                mock.Search<TimeSeriesSearchRequest, TimeSeriesSearchResponse>(
                    Arg.Matches<TimeSeriesSearchRequest>(req =>
                        req.SiteVariableAmountId == long.Parse(requestedSiteUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new TimeSeriesSearchResponse
            {
                Sites =
                [
                    new TimeSeriesSearchItem()
                    {
                        SiteVariableAmountId = requestedSiteUuid
                    }
                ],
                LastUuid = null
            });

        _formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse
            {
                Features =
                [
                    new OgcFeature
                    {
                        Id = requestedSiteUuid
                    }
                ],
                Links = []
            });

        // Act
        var handler = CreateTimeSeriesFeatureItemGetRequestHandler();
        var request = new TimeSeriesFeatureItemGetRequest
        {
            Id = requestedSiteUuid
        };

        var response = await handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Handler_FoundZeroSites_NotFoundErrorIsThrown()
    {
        // Arrange
        string requestedSiteUuid = "1234";
        _siteVariableAmountsAccessorMock.Arrange(mock =>
                mock.Search<TimeSeriesSearchRequest, TimeSeriesSearchResponse>(
                    Arg.Matches<TimeSeriesSearchRequest>(req =>
                        req.SiteVariableAmountId == long.Parse(requestedSiteUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new TimeSeriesSearchResponse
            {
                Sites = [],
                LastUuid = null
            });

        var formatEngineExpectation = _formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());

        // Act
        var handler = CreateTimeSeriesFeatureItemGetRequestHandler();
        var request = new TimeSeriesFeatureItemGetRequest
        {
            Id = requestedSiteUuid
        };

        var response = await handler.Handle(request);

        // Assert
        formatEngineExpectation.OccursNever();
        response.Error.Should().BeOfType<NotFoundError>();
    }

    [TestMethod]
    public async Task Handler_FoundMultipleSites_NotFoundErrorIsThrown()
    {
        // Arrange
        string requestedSiteUuid = "1234";
        _siteVariableAmountsAccessorMock.Arrange(mock =>
                mock.Search<TimeSeriesSearchRequest, TimeSeriesSearchResponse>(
                    Arg.Matches<TimeSeriesSearchRequest>(req =>
                        req.SiteVariableAmountId == long.Parse(requestedSiteUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new TimeSeriesSearchResponse
            {
                Sites = [new TimeSeriesSearchItem(), new TimeSeriesSearchItem()],
                LastUuid = null
            });

        var formatEngineExpectation = _formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());

        // Act
        var handler = CreateTimeSeriesFeatureItemGetRequestHandler();
        var request = new TimeSeriesFeatureItemGetRequest
        {
            Id = requestedSiteUuid
        };

        var response = await handler.Handle(request);

        // Assert
        formatEngineExpectation.OccursNever();
        response.Error.Should().BeOfType<NotFoundError>();
    }

    private TimeSeriesFeatureItemGetRequestHandler CreateTimeSeriesFeatureItemGetRequestHandler()
    {
        return new TimeSeriesFeatureItemGetRequestHandler(_siteVariableAmountsAccessorMock, _formattingEngine,
            NullLogger<TimeSeriesFeatureItemGetRequestHandler>.Instance);
    }
}