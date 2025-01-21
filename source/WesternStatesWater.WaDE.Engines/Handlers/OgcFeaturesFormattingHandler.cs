using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Common;
using WesternStatesWater.WaDE.Common.Contexts;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Utilities;
using Link = WesternStatesWater.WaDE.Engines.Contracts.Ogc.Link;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public class OgcFeaturesFormattingHandler(
    IConfiguration configuration,
    IContextUtility contextUtility
) : OgcFormattingHandlerBase(configuration),
    IRequestHandler<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>
{
    public Task<OgcFeaturesFormattingResponse> Handle(OgcFeaturesFormattingRequest request)
    {
        var requestUri = contextUtility.GetRequiredContext<ApiContext>().RequestUri;
        if (requestUri is null)
        {
            throw new WaDEException($"{nameof(ApiContext.RequestUri)} cannot be null.");
        }
        
        var features = request.Items
            .Select(item => new Feature
            {
                Geometry = item.Geometry,
                Attributes = BuildAttributesTable(item)
            })
            .ToArray();

        List<Link> links =
        [
            new()
            {
                Href = requestUri.ToString(), Rel = "self", Type = "application/json",
                Title = "This document as JSON"
            },
            new()
            {
                Href = $"{OgcHost}/collections/{request.CollectionId}", Rel = "collection", Type = "application/json",
                Title = "The collection metadata"
            }
        ];

        if (!string.IsNullOrWhiteSpace(request.LastUuid))
        {
            // Add the "next" parameter to the existing query string.
            var queryString = new QueryString(requestUri.Query);
            queryString = queryString.Add("next", request.LastUuid);
            links.Add(new Link { Href = $"{OgcHost}/collections/{request.CollectionId}/items{queryString.Value}", Rel = "next", Type = "application/geo+json", Title = "Next page of features" });
        }
        
        var response = new OgcFeaturesFormattingResponse
        {
            Features = features,
            Links = links.ToArray()
        };

        return Task.FromResult(response);
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