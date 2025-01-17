using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WesternStatesWater.WaDE.Engines.Tests;

[TestClass]
public class LinkBuilderTests
{
    [TestMethod]
    public void LinkBuilder_LandingPages_SupportedTypes()
    {
        var linkBuilder = new LinkBuilder(null)
            .AddLandingPage()
            .Build();

        linkBuilder.Should().HaveCount(2);
        linkBuilder.Select(x => x.Type).Should().BeEquivalentTo("text/html", "application/json");
        var htmlLandingPage = linkBuilder.First(lb => lb.Type == "text/html");
        htmlLandingPage.Href.Should().Be("http://localhost:7071/swagger/ui");
        htmlLandingPage.Rel.Should().Be("root");
        htmlLandingPage.Title.Should().NotBeEmpty();
        var jsonLandingPage = linkBuilder.First(lb => lb.Type == "application/json");
        jsonLandingPage.Href.Should().Be("http://localhost:7071/swagger.json");
        jsonLandingPage.Rel.Should().Be("root");
        jsonLandingPage.Title.Should().NotBeEmpty();
    }

    [TestMethod]
    public void LinkBuilder_AddCollections()
    {
        var linkBuilder = new LinkBuilder(null)
            .AddCollections()
            .Build();

        linkBuilder.Should().HaveCount(1);
        var collectionLink = linkBuilder[0];
        collectionLink.Href.Should().Be("http://localhost:7071/api/collections");
        collectionLink.Type.Should().Be("application/json");
        collectionLink.Rel.Should().Be("self");
        collectionLink.Title.Should().Be("This document as JSON");
    }

    [TestMethod]
    public void LinkBuilder_AddCollection()
    {
        var linkBuilder = new LinkBuilder(null)
            .AddCollection("sites")
            .Build();

        linkBuilder.Should().HaveCount(2);
        var collectionLink = linkBuilder[0];
        collectionLink.Href.Should().Be("http://localhost:7071/api/collections/sites");
        collectionLink.Type.Should().Be("application/json");
        collectionLink.Rel.Should().Be("self");

        var collectionItemsLinks = linkBuilder[1];
        collectionItemsLinks.Href.Should().Be("http://localhost:7071/api/collections/sites/items");
        collectionItemsLinks.Type.Should().Be("application/geo+json");
        collectionItemsLinks.Rel.Should().Be("items");
    }
}