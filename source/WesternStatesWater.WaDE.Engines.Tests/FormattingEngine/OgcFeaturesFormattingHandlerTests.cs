using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

[TestClass]
public class OgcFeaturesFormattingHandlerTests : OgcFormattingTestBase
{
    [TestMethod]
    public async Task Features_Required_Links()
    {
        MockApiContextRequest("/collections/test/items");
        
        var request = new OgcFeaturesFormattingRequest()
        {
            Items = []
        };

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Links.Should().NotBeNull();
        response.Links.Should().Contain(link => link.Rel == "self" && link.Type == "application/json");
        response.Links.Should().Contain(link => link.Rel == "alternate" && link.Type == "application/geo+json");
    }

    [TestMethod]
    public async Task Features_HaveNoItems_ReturnsNoNextLink()
    {
        MockApiContextRequest("/collections/test/items");
        
        var request = new OgcFeaturesFormattingRequest()
        {
            Items = []
        };

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Links.Should().NotBeNull();
        response.Links.Should().NotContain(link => link.Rel == "next");
    }

    [TestMethod]
    public async Task SiteFeature_HasItems_IncludesNextPageLink()
    {
        // Arrange
        MockApiContextRequest("/collections/sites/items");
        var request = new OgcFeaturesFormattingRequest()
        {
            Items =
            [
                new SiteFeature
                {
                    Id = "site-id",
                    Geometry = Point.Empty
                }
            ],
            LastUuid = "site-id"
        };

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Links.Should().NotBeNull();
        response.Links.Should().ContainEquivalentOf(
            new Link
            {
                Href = $"{ApiHostName}/collections/sites/items?next=site-id",
                Rel = "next",
                Title = "Next page of features",
                Type = "application/geo+json"
            }
        );
    }

    private OgcFeaturesFormattingHandler CreateHandler()
    {
        return new OgcFeaturesFormattingHandler(
            Configuration.GetConfiguration(), 
            _contextUtilityMock);
    }
}