using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers.V2;

[TestClass]
public class CollectionMetadataGetRequestHandlerTests
{
    private readonly IFormattingEngine _formattingEngineMock = Mock.Create<IFormattingEngine>(Behavior.Strict);

    [TestMethod]
    public async Task Handler_MapsRequest_ShouldReturnResponse()
    {
        // Arrange
        Uri requestUri = new Uri("https://localhost/api/v2/collections/foo") ;
        var request = new CollectionMetadataGetRequest
        {
            RequestUri = requestUri
        };
        
        var mockResponse = new CollectionResponse
        {
            Collection = new Collection
            {
                Id = "foo",
                Extent = new Extent
                {
                    Spatial = new Spatial
                    {
                        Bbox = [[-10, -10, 10, 10]],
                        Crs = "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
                    },
                    Temporal = new Temporal
                    {
                        Interval = [["2024-01-01T00:00:00Z", "2024-12-31T23:59:59Z"]],
                        Trs = "http://www.opengis.net/def/uom/ISO-8601/0/Gregorian",
                    }
                },
                Links = [
                new Link
                {
                    Href = "http://localhost/",
                    Rel = "self",
                }]
            }
        };
        _formattingEngineMock.Arrange(mock =>
                mock.Format<CollectionRequest, CollectionResponse>(
                    Arg.Matches<CollectionRequest>(req => req.RequestUri == requestUri)))
            .ReturnsAsync(mockResponse);

        // Act
        var handler = CreateHandler();
        var result = await handler.Handle(request);

        // Assert
        result.Collection.Should().NotBeNull();
        result.Collection.Id.Should().Be("foo");
        result.Collection.Extent.Should().NotBeNull();
        result.Collection.Extent!.Spatial.Should().NotBeNull();
        result.Collection.Extent.Spatial.Bbox[0][0].Should().Be(-10);
        result.Collection.Extent.Spatial.Bbox[0][1].Should().Be(-10);
        result.Collection.Extent.Spatial.Bbox[0][2].Should().Be(10);
        result.Collection.Extent.Spatial.Bbox[0][3].Should().Be(10);
        result.Collection.Extent.Spatial.Crs.Should().Be("http://www.opengis.net/def/crs/OGC/1.3/CRS84");
        result.Collection.Extent.Temporal.Should().NotBeNull();
        result.Collection.Extent.Temporal.Interval[0][0].Should().Be("2024-01-01T00:00:00Z");
        result.Collection.Extent.Temporal.Interval[0][1].Should().Be("2024-12-31T23:59:59Z");
        result.Collection.Extent.Temporal.Trs.Should().Be("http://www.opengis.net/def/uom/ISO-8601/0/Gregorian");
        result.Collection.Links.Should().NotBeNull();
        result.Collection.Links.Should().HaveCount(1);
        result.Collection.Links[0].Href.Should().Be("http://localhost/");
        result.Collection.Links[0].Rel.Should().Be("self");
        _formattingEngineMock.AssertAll();
    }

    private CollectionMetadataGetRequestHandler CreateHandler()
    {
        return new CollectionMetadataGetRequestHandler(_formattingEngineMock);
    }
}