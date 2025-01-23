using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.Shared.Errors;
using WesternStatesWater.Shared.Exceptions;
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
public class OverlayFeatureItemGetRequestHandlerTests
{
    private IRegulatoryOverlayAccessor _overlayAccessorMock = Mock.Create<IRegulatoryOverlayAccessor>();
    private IFormattingEngine _formattingEngine = Mock.Create<IFormattingEngine>();

    [TestMethod]
    public async Task Handler_FoundOneOverlays_ReturnsResponse()
    {
        // Arrange
        string requestedOverlayUuid = "NE123_abc";
        _overlayAccessorMock.Arrange(mock =>
                mock.Search<OverlaySearchRequest, OverlaySearchResponse>(
                    Arg.Matches<OverlaySearchRequest>(req =>
                        req.OverlayUuids.Contains(requestedOverlayUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new OverlaySearchResponse
            {
                Overlays =
                [
                    new OverlaySearchItem()
                    {
                        OverlayUuid = requestedOverlayUuid
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
                        Id = requestedOverlayUuid
                    }
                ],
                Links = []
            });

        // Act
        var handler = CreateOverlayFeatureItemGetRequestHandler();
        var request = new OverlayFeatureItemGetRequest
        {
            Id = requestedOverlayUuid
        };

        var response = await handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.Feature.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Handler_FoundZeroOverlays_NotFoundErrorIsThrown()
    {
        // Arrange
        string requestedOverlayUuid = "NE123_abc";
        _overlayAccessorMock.Arrange(mock =>
                mock.Search<OverlaySearchRequest, OverlaySearchResponse>(
                    Arg.Matches<OverlaySearchRequest>(req =>
                        req.OverlayUuids.Contains(requestedOverlayUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new OverlaySearchResponse
            {
                Overlays = [],
                LastUuid = null
            });

        var formatEngineExpectation = _formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());

        // Act
        var handler = CreateOverlayFeatureItemGetRequestHandler();
        var request = new OverlayFeatureItemGetRequest
        {
            Id = requestedOverlayUuid
        };

        var response = await handler.Handle(request);
        
        // Assert
        formatEngineExpectation.OccursNever();
        response.Feature.Should().BeNull();
        response.Error.Should().BeOfType<NotFoundError>();
    }

    [TestMethod]
    public async Task Handler_FoundMultipleOverlays_NotFoundErrorIsThrown()
    {
        // Arrange
        string requestedOverlayUuid = "NE123_abc";
        _overlayAccessorMock.Arrange(mock =>
                mock.Search<OverlaySearchRequest, OverlaySearchResponse>(
                    Arg.Matches<OverlaySearchRequest>(req =>
                        req.OverlayUuids.Contains(requestedOverlayUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new OverlaySearchResponse
            {
                Overlays = [new OverlaySearchItem(), new OverlaySearchItem()],
                LastUuid = null
            });

        var formatEngineExpectation = _formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());

        // Act
        var handler = CreateOverlayFeatureItemGetRequestHandler();
        var request = new OverlayFeatureItemGetRequest
        {
            Id = requestedOverlayUuid
        };

        var response = await handler.Handle(request);
        
        // Assert
        formatEngineExpectation.OccursNever();
        response.Feature.Should().BeNull();
        response.Error.Should().BeOfType<NotFoundError>();
    }

    private OverlayFeatureItemGetRequestHandler CreateOverlayFeatureItemGetRequestHandler()
    {
        return new OverlayFeatureItemGetRequestHandler(_overlayAccessorMock, _formattingEngine, NullLogger<OverlayFeatureItemGetRequestHandler>.Instance);
    }
}