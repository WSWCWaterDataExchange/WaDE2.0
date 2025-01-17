using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Common.Context;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Utilities;
using Link = WesternStatesWater.WaDE.Engines.Contracts.Ogc.Link;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public class OgcFeaturesFormattingHandler(IConfiguration configuration, IContextUtility contextUtility) : OgcFormattingHandlerBase(configuration, contextUtility),
    IRequestHandler<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>
{
    public Task<OgcFeaturesFormattingResponse> Handle(OgcFeaturesFormattingRequest request)
    {
        var apiContext = contextUtility.GetRequiredContext<ApiContext>();
        var features = request.Items
            .Select(item => new Feature
            {
                Geometry = item.Geometry,
                Attributes = BuildAttributesTable(item)
            })
            .ToArray();

        var links = BuildLinks(request);

        var response = new OgcFeaturesFormattingResponse
        {
            Features = features,
            Links = links
        };

        return Task.FromResult(response);
    }

    private Link[] BuildLinks(OgcFeaturesFormattingRequest request)
    {
        var links = new LinkBuilder(null)
            .AddLandingPage();

        if (request.LastUuid is not null)
        {
            links.AddNextFeatures(request.CollectionId, request.LastUuid);
        }

        return links.Build();
    }

    /// <summary>
    /// Builds a GeoJson Feature "properties" using the <see cref="JsonPropertyNameAttribute"/> as the property name.
    /// </summary>
    /// <param name="item">Feature item</param>
    /// <returns>Creates an AttributeTable from the derived FeatureBase type. Geometry is omitted from the table.</returns>
    private static AttributesTable BuildAttributesTable(FeatureBase item)
    {
        var properties = new AttributesTable();
        foreach (var property in item.GetType().GetProperties()
                     .Where(prop => prop.GetCustomAttribute<JsonPropertyNameAttribute>() is not null))
        {
            var attrName = property.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name;

            properties.Add(attrName, property.GetValue(item));
        }

        return properties;
    }
}