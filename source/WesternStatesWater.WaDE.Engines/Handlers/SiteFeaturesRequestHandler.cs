using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

namespace WesternStatesWater.WaDE.Engines.Handlers;

internal class SiteFeaturesRequestHandler(
    IConfiguration configuration,
    ISiteAccessor siteAccessor
) : OgcFormattingHandlerBase(configuration),
    IRequestHandler<SiteFeaturesRequest, SiteFeaturesResponse>
{
    public async Task<SiteFeaturesResponse> Handle(SiteFeaturesRequest request)
    {
        var searchRequest = new SiteSearchRequest
        {
            FilterBoundary = request.BoundingBox != null ? ConvertBoundaryBoxToPolygon(request.BoundingBox) : null,
            LastSiteUuid = request.LastSiteUuid,
            Limit = request.Limit
        };
        var searchResponse = await siteAccessor.Search<SiteSearchRequest, SiteSearchResponse>(searchRequest);

        List<Feature> features = [];
        features.AddRange(searchResponse.Sites.Select(site => new Feature
        {
            Geometry = site.Location,
            Attributes = new AttributesTable()
            {
                { "id", site.SiteUuid },
                { "nId", site.SiteNativeId },
                { "name", site.SiteName },
                { "usgsSiteId", site.UsgsSiteId },
                { "sType", site.SiteType },
                { "coordMethod", site.CoordinateMethod },
                { "coordAcc", site.CoordinateAccuracy },
                { "gnisCode", site.GnisCode },
                { "epsgCode", site.EpsgCode },
                { "nhdNetStat", site.NhdNetworkStatus },
                { "nhdProd", site.NhdProduct },
                { "state", site.State },
                { "huc8", site.Huc8 },
                { "huc12", site.Huc12 },
                { "county", site.County },
                { "rightUuids", site.RightUuids },
                { "isTimeSeries", site.IsTimeSeries },
                { "podOrPouSite", site.PodOrPouSite },
                { "waterSources", site.WaterSources },
                { "overlays", site.Overlays }
            }
        }));
        
        var links = new LinkBuilder(ServerUrl, ApiPath)
            .AddLandingPage();

        if (searchResponse.LastUuid is not null)
        {
            links.AddNextFeatures(Constants.SitesCollectionId, searchResponse.LastUuid);
        }
        
        return new SiteFeaturesResponse
        {
            Features = features.ToArray(),
            Links = links.Build()
        };
    }
}