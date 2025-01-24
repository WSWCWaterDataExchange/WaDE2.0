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
public class RightFeatureItemGetRequestHandlerTests
{
    private IWaterAllocationAccessor _allocationAccessorMock = Mock.Create<IWaterAllocationAccessor>();
    private IFormattingEngine _formattingEngine = Mock.Create<IFormattingEngine>();

    [TestMethod]
    public async Task Handler_FoundOneAllocation_ReturnsResponse()
    {
        // Arrange
        string requestedAllocationUuid = "NE123_abc";
        _allocationAccessorMock.Arrange(mock =>
                mock.Search<AllocationSearchRequest, AllocationSearchResponse>(
                    Arg.Matches<AllocationSearchRequest>(req =>
                        req.AllocationUuid.Contains(requestedAllocationUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new AllocationSearchResponse
            {
                Allocations = 
                [
                    new AllocationSearchItem()
                    {
                        AllocationUUID = requestedAllocationUuid
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
                        Id = requestedAllocationUuid
                    }
                ],
                Links = []
            });

        // Act
        var handler = CreateRightFeatureItemGetRequestHandler();
        var request = new RightFeatureItemGetRequest
        {
            Id = requestedAllocationUuid
        };

        var response = await handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.Feature.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Handler_FoundZeroAllocations_NotFoundErrorIsThrown()
    {
        // Arrange
        string requestedAllocationUuid = "NE123_abc";
        _allocationAccessorMock.Arrange(mock =>
                mock.Search<AllocationSearchRequest, AllocationSearchResponse>(
                    Arg.Matches<AllocationSearchRequest>(req =>
                        req.AllocationUuid.Contains(requestedAllocationUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new AllocationSearchResponse
            {
                Allocations = [],
                LastUuid = null
            });

        var formatEngineExpectation = _formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());

        // Act
        var handler = CreateRightFeatureItemGetRequestHandler();
        var request = new RightFeatureItemGetRequest
        {
            Id = requestedAllocationUuid
        };

        var response = await handler.Handle(request);
        
        // Assert
        formatEngineExpectation.OccursNever();
        response.Feature.Should().BeNull();
        response.Error.Should().BeOfType<NotFoundError>();
    }

    [TestMethod]
    public async Task Handler_FoundMultipleAllocations_NotFoundErrorIsThrown()
    {
        // Arrange
        string requestedAllocationUuid = "NE123_abc";
        _allocationAccessorMock.Arrange(mock =>
                mock.Search<AllocationSearchRequest, AllocationSearchResponse>(
                    Arg.Matches<AllocationSearchRequest>(req =>
                        req.AllocationUuid.Contains(requestedAllocationUuid) &&
                        req.Limit == 1)))
            .ReturnsAsync(new AllocationSearchResponse
            {
                Allocations = [new AllocationSearchItem(), new AllocationSearchItem()],
                LastUuid = null
            });

        var formatEngineExpectation = _formattingEngine.Arrange(mock =>
                mock.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(
                    Arg.IsAny<OgcFeaturesFormattingRequest>()))
            .ReturnsAsync(new OgcFeaturesFormattingResponse());

        // Act
        var handler = CreateRightFeatureItemGetRequestHandler();
        var request = new RightFeatureItemGetRequest
        {
            Id = requestedAllocationUuid
        };

        var response = await handler.Handle(request);
        
        // Assert
        formatEngineExpectation.OccursNever();
        response.Feature.Should().BeNull();
        response.Error.Should().BeOfType<NotFoundError>();
    }

    private RightFeatureItemGetRequestHandler CreateRightFeatureItemGetRequestHandler()
    {
        return new RightFeatureItemGetRequestHandler(_allocationAccessorMock, _formattingEngine, NullLogger<RightFeatureItemGetRequestHandler>.Instance);
    }
}