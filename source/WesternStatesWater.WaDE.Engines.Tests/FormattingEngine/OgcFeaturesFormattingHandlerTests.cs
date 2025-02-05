using System.Reflection;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using Telerik.JustMock;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

[TestClass]
public class OgcFeaturesFormattingHandlerTests : OgcFormattingTestBase
{
    [TestMethod]
    public void Features_AllPropertiesExceptGeometry_ShouldHaveJsonPropertyNameAttribute()
    {
        var featureTypes = typeof(FeatureBase).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(FeatureBase)))
            .ToArray();
    
        // Smoke test to make sure we have some feature types.
        featureTypes.Should().NotBeEmpty();
    
        foreach (var featureType in featureTypes)
        {
            Console.WriteLine($"Checking {featureType.Name}");
    
            var featureProperties = featureType.GetProperties();
            var geometryProp = featureProperties.Single(prop => prop.Name == nameof(FeatureBase.Geometry));
    
            geometryProp
                .GetCustomAttribute<JsonPropertyNameAttribute>()
                .Should()
                .BeNull(
                    $"property '{geometryProp.Name}' on '{featureType.Name}' should not have a {nameof(JsonPropertyNameAttribute)}."
                );
    
            featureProperties.Except([geometryProp])
                .Select(prop => new
                {
                    Attribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>(),
                    PropertyName = prop.Name
                })
                .Should()
                .AllSatisfy(property =>
                    property.Attribute.Should().NotBeNull(
                        $"property '{property.PropertyName}' on '{featureType.Name}' should have a {nameof(JsonPropertyNameAttribute)}."
                    )
                );
        }
    }

    [TestMethod]
    public async Task PropertiesWithFeaturePropertyNameAttribute_ShouldAddValuesToAttributes()
    {
        MockApiContextRequest("/collections/test/items");
        var feature = new TestFeature
        {
            StringProperty = "string!",
            NullableStringProperty = "nullable string!",
            IntProperty = 1,
            NullableIntProperty = 2,
            DoubleProperty = 3.0,
            NullableDoubleProperty = 4.0,
            BoolProperty = true,
            NullableBoolProperty = false,
            StringArrayProperty = ["string array!"]
        };

        var request = new OgcFeaturesFormattingRequest { Items = [feature] };
        var response = await CreateHandler().Handle(request);

        // All attributes are keyed off the FeaturePropertyNameAttributes on the class properties.
        response.Features[0].Properties["sp"].Should().Be("string!");
        response.Features[0].Properties["nsp"].Should().Be("nullable string!");
        response.Features[0].Properties["ip"].Should().Be(1);
        response.Features[0].Properties["nip"].Should().Be(2);
        response.Features[0].Properties["dp"].Should().Be(3.0);
        response.Features[0].Properties["ndp"].Should().Be(4.0);
        response.Features[0].Properties["bp"].Should().Be(true);
        response.Features[0].Properties["nbp"].Should().Be(false);
        response.Features[0].Properties["sap"].Should().BeEquivalentTo(new[] { "string array!" });
    }

    [TestMethod]
    public async Task PropertiesWithFeaturePropertyNameAttribute_NullableAttributesNull_ShouldBeAddedAsNull()
    {
        MockApiContextRequest("/collections/test/items");
        
        var feature = new TestFeature
        {
            StringProperty = "string!",
            IntProperty = 1,
            DoubleProperty = 2.0,
            BoolProperty = true
        };

        var request = new OgcFeaturesFormattingRequest { Items = [feature] };
        var response = await CreateHandler().Handle(request);

        // All attributes are keyed off the FeaturePropertyNameAttributes on the class properties.
        response.Features[0].Properties["sp"].Should().Be("string!");
        response.Features[0].Properties["nsp"].Should().BeNull();
        response.Features[0].Properties["ip"].Should().Be(1);
        response.Features[0].Properties["nip"].Should().BeNull();
        response.Features[0].Properties["dp"].Should().Be(2.0);
        response.Features[0].Properties["ndp"].Should().BeNull();
        response.Features[0].Properties["bp"].Should().Be(true);
        response.Features[0].Properties["nbp"].Should().BeNull();
        response.Features[0].Properties["sap"].Should().BeNull();
    }

    [TestMethod]
    public async Task AttributesTableContainsAllPropertiesWithJsonPropertyNameAttribute()
    {
        MockApiContextRequest("/collections/test/items");
        
        var properties = typeof(TestFeature).GetProperties();
        var namedProperties = properties
            .Where(prop => prop.GetCustomAttribute<JsonPropertyNameAttribute>() is not null)
            .ToArray();
    
        // All the named properties + Geometry = 11
        properties.Length.Should().Be(11);
    
        // 10, minus the geometry property
        namedProperties.Length.Should().Be(10);
    
        var feature = new TestFeature();
        var request = new OgcFeaturesFormattingRequest { Items = [feature] };
        var response = await CreateHandler().Handle(request);
    
        response.Features[0].Properties.Count.Should().Be(namedProperties.Length);
    }
    
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

public class TestFeature : FeatureBase
{
    [JsonPropertyName("sp")]
    public string StringProperty { get; init; } = null!;

    [JsonPropertyName("nsp")]
    public string? NullableStringProperty { get; init; }

    [JsonPropertyName("ip")]
    public int IntProperty { get; init; }

    [JsonPropertyName("nip")]
    public int? NullableIntProperty { get; init; }

    [JsonPropertyName("dp")]
    public double DoubleProperty { get; init; }

    [JsonPropertyName("ndp")]
    public double? NullableDoubleProperty { get; init; }

    [JsonPropertyName("bp")]
    public bool BoolProperty { get; init; }

    [JsonPropertyName("nbp")]
    public bool? NullableBoolProperty { get; init; }

    [JsonPropertyName("sap")]
    public string[] StringArrayProperty { get; init; } = null!;
}