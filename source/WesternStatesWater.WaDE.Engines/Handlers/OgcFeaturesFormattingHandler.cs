using System.Reflection;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Attributes;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using Link = WesternStatesWater.WaDE.Engines.Contracts.Ogc.Link;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public class OgcFeaturesFormattingHandler(IConfiguration configuration) : OgcFormattingHandlerBase(configuration),
    IRequestHandler<FeaturesRequest, FeaturesResponse>
{
    public Task<FeaturesResponse> Handle(FeaturesRequest request)
    {
        List<Feature> features = [];
        features.AddRange(request.Items.Select(item => new Feature
        {
            Geometry = item.Geometry,
            Attributes = BuildAttributesTable(item)
        }));
        
        return Task.FromResult(new FeaturesResponse
        {
            Features = features.ToArray(),
            Links = BuildLinks(request)
        });
    }

    private Link[] BuildLinks(FeaturesRequest request)
    {
        var links = new LinkBuilder(ServerUrl, ApiPath)
            .AddLandingPage();
        
        if (request.LastUuid is not null)
        {
            links.AddNextFeatures(Constants.SitesCollectionId, request.LastUuid);
        }

        return links.Build();
    }

    /// <summary>
    /// Builds a GeoJson Feature "properties" using the <see cref="FeaturePropertyNameAttribute"/> as the property name.
    /// </summary>
    /// <param name="item">Feature item</param>
    /// <returns>Creates an AttributeTable from the derived FeatureBase type. Geometry is omitted from the table.</returns>
    private static AttributesTable BuildAttributesTable(FeatureBase item)
    {
        var properties = new AttributesTable();
        foreach (var property in item.GetType().GetProperties().Where(prop => prop.GetCustomAttribute<FeaturePropertyNameAttribute>() is not null))
        {
            var attrName = property.GetCustomAttribute<FeaturePropertyNameAttribute>()!.GetName();
            
            properties.Add(attrName, property.GetValue(item));
        }

        return properties;
    }
}