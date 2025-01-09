using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Attributes;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

[TestClass]
public class OgcFeaturesFormattingHandlerTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        Environment.SetEnvironmentVariable("ServerUrl", "http://localhost");
        Environment.SetEnvironmentVariable("ApiPath", "/api");
    }

    [DataTestMethod]
    [DataRow(typeof(SiteFeature))]
    [DataRow(typeof(OverlayFeature))]
    public async Task Features_Items_PropertiesHaveFeaturePropertyNameAttribute(Type itemType)
    {
        // Arrange
        var item = (FeatureBase) Activator.CreateInstance(itemType)!;
        foreach (var property in item.GetType().GetProperties())
        {
            if (property.Name == nameof(FeatureBase.Geometry))
            {
                // Geometry is not part of properties.
                property.SetValue(item, Point.Empty);
                continue;
            }

            if (property.PropertyType == typeof(string))
            {
                property.SetValue(item, property.Name);
            }
            else if (property.PropertyType == typeof(string[]))
            {
                property.SetValue(item, (string[]) [property.Name]);
            }
            else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {
                property.SetValue(item, true);
            }
            else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
            {
                property.SetValue(item, (double) item.GetHashCode());
            }
            else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {
                property.SetValue(item, item.GetHashCode());
            }
        }

        var request = new FeaturesRequest()
        {
            Items =
            [
                item
            ]
        };

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Features.Should().HaveCount(1);
        var feature = response.Features[0];
        feature.Geometry.Should().Be(Point.Empty);
        foreach (var property in item.GetType().GetProperties())
        {
            if (property.Name == nameof(FeatureBase.Geometry))
            {
                continue;
            }

            var attrName = property.GetCustomAttribute<FeaturePropertyNameAttribute>()?.GetName();
            if (property.PropertyType.IsArray)
            {
                feature.Attributes[attrName].Should().BeEquivalentTo(new[] { property.Name });
            }
            else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {
                feature.Attributes[attrName].Should().Be(true);
            }
            else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
            {
                feature.Attributes[attrName].Should().Be((double) item.GetHashCode());
            }
            else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {
                feature.Attributes[attrName].Should().Be(item.GetHashCode());
            }
            else
            {
                feature.Attributes[attrName].Should().Be(property.Name);
            }
        }
    }

    [TestMethod]
    public async Task Features_Items_PropertyMissingFeaturePropertyNameAttribute()
    {
        // Arrange
        var item = new BadFeature
        {
            MissingAttribute = "does-not-matter"
        };
        var request = new FeaturesRequest()
        {
            Items =
            [
                item
            ]
        };

        // Act
        var handler = CreateHandler();
        Func<Task> act = async () => await handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(
                $"{item.GetType()} property {nameof(BadFeature.MissingAttribute)} is missing {nameof(FeaturePropertyNameAttribute)}.");
    }

    [TestMethod]
    public async Task Features_HaveNoItems_ReturnsNoNextLink()
    {
        var request = new FeaturesRequest()
        {
            Items = []
        };
        
        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);
        
        // Assert
        response.Links.Should().NotBeNull();
        response.Links.Should().HaveCount(2);
        response.Links.Should().BeEquivalentTo([
            new Link
            {
                Href = "http://localhost/swagger/ui",
                Rel = "root",
                Title = "Landing page",
                Type = "text/html"
            },
            new Link
            {
                Href = "http://localhost/swagger.json",
                Rel = "root",
                Title = "Landing page",
                Type = "application/json"
            }
        ]);
    }

    [TestMethod]
    public async Task SiteFeature_HasItems_IncludesNextPageLink()
    {
        // Arrange
        var request = new FeaturesRequest()
        {
            Items =
            [
                new SiteFeature
                {
                    Id = "site-id",
                    Geometry = Point.Empty
                }
            ]
        };

        // Act
        var handler = CreateHandler();
        var response = await handler.Handle(request);

        // Assert
        response.Links.Should().NotBeNull();
        response.Links.Should().HaveCount(3);
        response.Links.Should().BeEquivalentTo([
            new Link
            {
                Href = "http://localhost/swagger/ui",
                Rel = "root",
                Title = "Landing page",
                Type = "text/html"
            },
            new Link
            {
                Href = "http://localhost/swagger.json",
                Rel = "root",
                Title = "Landing page",
                Type = "application/json"
            },
            new Link
            {
                Href = "http://localhost/api/collections/sites/items?next=site-id",
                Rel = "next",
                Title = null,
                Type = "application/geo+json"
            }
        ]);
    }

    private OgcFeaturesFormattingHandler CreateHandler()
    {
        return new OgcFeaturesFormattingHandler(Configuration.GetConfiguration());
    }
}

public class BadFeature : FeatureBase
{
    public string MissingAttribute { get; set; }
}