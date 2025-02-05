using System.Reflection;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using WesternStatesWater.Shared.Exceptions;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Common;
using WesternStatesWater.WaDE.Common.Contexts;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Utilities;

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
            .Select(item => CreateFeature(item, GetCollectionId(requestUri)))
            .ToArray();

        var response = new OgcFeaturesFormattingResponse
        {
            Features = features,
            Links = GetFeaturesCollectionLinks(request, requestUri)
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

    /// <summary>
    /// Creates an <see cref="Contracts.Ogc.OgcFeature"/> from a <see cref="FeatureBase"/> item.
    /// </summary>
    /// <param name="item">Feature Base</param>
    /// <param name="collectionId">OGC Collection Id</param>
    /// <returns></returns>
    private OgcFeature CreateFeature(FeatureBase item, string collectionId)
    {
        List<Link> featureLinks =
        [
            new()
            {
                Href = $"{OgcHost}/collections/{collectionId}/items/{item.Id}",
                Rel = "self",
                Type = ContentTypeJson,
                Title = "This feature as JSON"
            },
            new()
            {
                Href = $"{OgcHost}/collections/{collectionId}/items/{item.Id}",
                Rel = "alternate",
                Type = "application/geo+json",
                Title = "This feature as JSON"
            },
            new()
            {
                Href = $"{OgcHost}/collections/{collectionId}/items",
                Rel = "items",
                Type = ContentTypeJson,
                Title = "The features in this collection"
            },
            new()
            {
                Href = $"{OgcHost}/collections/{collectionId}",
                Rel = "collection",
                Type = ContentTypeJson,
                Title = "The collection metadata"
            }
        ];
            
        featureLinks.AddRange(GetFeatureRelatedLinks(item));

        return new OgcFeature
        {
            Id = item.Id,
            Geometry = item.Geometry,
            Properties = BuildAttributesTable(item),
            Links = featureLinks.ToArray()
        };
    }

    private Link[] GetFeaturesCollectionLinks(OgcFeaturesFormattingRequest request, Uri requestUri)
    {
        List<Link> links =
        [
            new()
            {
                Href = $"{OgcHost}/collections/{GetCollectionId(requestUri)}/items", Rel = "self", Type = ContentTypeJson,
                Title = "This document as JSON"
            },
            new()
            {
                Href = $"{OgcHost}/collections/{GetCollectionId(requestUri)}/items", 
                Rel = "alternate", 
                Type = "application/geo+json",
                Title = "This document as JSON"
            }
        ];

        if (!string.IsNullOrWhiteSpace(request.LastUuid))
        {
            // Add the "next" parameter to the existing query string.
            var queryString = HttpUtility.ParseQueryString(requestUri.Query);
            queryString["next"] = request.LastUuid;
            links.Add(new Link
            {
                Href = $"{requestUri.GetLeftPart(UriPartial.Path)}?{queryString}", Rel = "next",
                Type = "application/geo+json", Title = "Next page of features"
            });
        }

        return links.ToArray();
    }

    private Link[] GetFeatureRelatedLinks(FeatureBase featureBase)
    {
        return featureBase switch
        {
            SiteFeature siteFeature => SiteFeatureRelatedLinks(siteFeature),
            RightFeature rightFeature => RightFeatureRelatedLinks(rightFeature),
            OverlayFeature overlayFeature => OverlayFeatureRelatedLinks(overlayFeature),
            _ => []
        };
    }

    private Link[] SiteFeatureRelatedLinks(SiteFeature feature)
    {
        return
        [
            new Link
            {
                Href = $"{OgcHost}/collections/rights/items?siteuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The rights associated with this site"
            },
            new Link
            {
                Href = $"{OgcHost}/collections/overlays/items?siteuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The overlays associated with this site"
            },
            new Link
            {
                Href = $"{OgcHost}/collections/timeseries/items?siteuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "Time series data for this site"
            }
        ];
    }

    private Link[] RightFeatureRelatedLinks(RightFeature feature)
    {
        return
        [
            new Link
            {
                Href = $"{OgcHost}/collections/sites/items?allocationUuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The sites associated with this water right"
            },
            new Link
            {
                Href = $"{OgcHost}/collections/overlays/items?allocationUuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The overlays associated with this water right"
            }
        ];
    }

    private Link[] OverlayFeatureRelatedLinks(OverlayFeature feature)
    {
        return
        [
            new Link
            {
                Href = $"{OgcHost}/collections/sites/items?overlayuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The sites associated with this overlay"
            },
            new Link
            {
                Href = $"{OgcHost}/collections/rights/items?overlayuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The rights associated with this overlay"
            }
        ];
    }

    private Link[] TimeSeriesRelatedLinks(TimeSeriesFeature feature)
    {
        return
        [
            new Link
            {
                Href = $"{OgcHost}/collections/sites/items?siteuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "Site"
            },
            new Link
            {
                Href = $"{OgcHost}/collections/rights/items?siteuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The rights associated with this site"
            },
            new Link
            {
                Href = $"{OgcHost}/collections/overlays/items?siteuuids={feature.Id}", Rel = "related",
                Type = ContentTypeJson, Title = "The overlays associated with this site"
            }
        ];
    }
}