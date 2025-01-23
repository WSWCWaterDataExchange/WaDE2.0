using System;
using System.Collections.Generic;
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
public class SiteFeatureItemGetRequestHandlerTests
{
    private ISiteAccessor _siteAccessorMock = Mock.Create<ISiteAccessor>();
    private IFormattingEngine _formattingEngine = Mock.Create<IFormattingEngine>();
    
    [TestMethod]
    public async Task Handler_FoundOneSite_ReturnsResponse()
    {
        // Arrange
        string requestedSiteUuid = "NE123_abc";
        _siteAccessorMock.Arrange(mock => 
                mock.Search<SiteSearchRequest, SiteSearchResponse>(
                    Arg.Matches<SiteSearchRequest>(req => 
                        req.SiteUuids.Contains(requestedSiteUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new SiteSearchResponse
            {
                Sites =
                [
                    new SiteSearchItem()
                    {
                        SiteUuid = requestedSiteUuid
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
        var handler = CreateSiteFeatureItemGetRequestHandler();
        var request = new SiteFeatureItemGetRequest
        {
            Id = requestedSiteUuid
        };
        
        var response = await handler.Handle(request);
        
        // Assert
        response.Should().NotBeNull();
        response.Feature.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Handler_FoundZeroSites_WaDENotFoundExceptionIsThrown()
    {
        // Arrange
        string requestedSiteUuid = "NE123_abc";
        _siteAccessorMock.Arrange(mock => 
                mock.Search<SiteSearchRequest, SiteSearchResponse>(
                    Arg.Matches<SiteSearchRequest>(req => 
                        req.SiteUuids.Contains(requestedSiteUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new SiteSearchResponse
            {
                Sites = [],
                LastUuid = null
            });

        var formatEngineExpectation =_formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());
        
        // Act
        var handler = CreateSiteFeatureItemGetRequestHandler();
        var request = new SiteFeatureItemGetRequest
        {
            Id = requestedSiteUuid
        };

        var response = await handler.Handle(request);
        
        // Assert
        formatEngineExpectation.OccursNever();
        response.Feature.Should().BeNull();
        response.Error.Should().BeOfType<NotFoundError>();
    }
    
    [TestMethod]
    public async Task Handler_FoundMultipleSites_WaDENotFoundExceptionIsThrown()
    {
        // Arrange
        string requestedSiteUuid = "NE123_abc";
        _siteAccessorMock.Arrange(mock => 
                mock.Search<SiteSearchRequest, SiteSearchResponse>(
                    Arg.Matches<SiteSearchRequest>(req => 
                        req.SiteUuids.Contains(requestedSiteUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new SiteSearchResponse
            {
                Sites = [new SiteSearchItem(), new SiteSearchItem()],
                LastUuid = null
            });

        var formatEngineExpectation =_formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());
        
        // Act
        var handler = CreateSiteFeatureItemGetRequestHandler();
        var request = new SiteFeatureItemGetRequest
        {
            Id = requestedSiteUuid
        };

        var response = await handler.Handle(request);
        
        // Assert
        formatEngineExpectation.OccursNever();
        response.Feature.Should().BeNull();
        response.Error.Should().BeOfType<NotFoundError>();
    }
    
    private SiteFeatureItemGetRequestHandler CreateSiteFeatureItemGetRequestHandler()
    {
        return new SiteFeatureItemGetRequestHandler(_siteAccessorMock, _formattingEngine, NullLogger<SiteFeatureItemGetRequestHandler>.Instance);
    }
}