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

        if (searchResponse.Sites.Count > 0)
        {
            links.AddNextFeatures(Constants.SitesCollectionId, searchResponse.Sites[^1].SiteUuid);
        }
        
        return new SiteFeaturesResponse
        {
            Features = features.ToArray(),
            Links = links.Build()
        };
    }
    
    /// <summary>
    /// Creates a Geometry from the bounding box. If the bounding box crosses the anti meridian, a multi polygon is created, else a single polygon is created.
    /// </summary>
    /// <param name="bbox">Bounding box that only supports 4 coordinates. [[minX,minY,maxX,maxY]]</param>
    /// <returns>SRID 4326 Polygon or MultiPolygon</returns>
    private Geometry? ConvertBoundaryBoxToPolygon(double[][]? bbox)
    {
        if (bbox is not { Length: 1 } || bbox[0].Length != 4) return null;
        
        double left = bbox[0][0];
        double bottom = bbox[0][1];
        double right = bbox[0][2];
        double top = bbox[0][3];

        if (left > right) // Crosses the anti meridian
        {
            var box1 = GeometryFactory.CreatePolygon([
                new Coordinate(left, top),
                new Coordinate(180, top),
                new Coordinate(180, bottom),
                new Coordinate(left, bottom),
                new Coordinate(left, top)
            ]);

            var box2 = GeometryFactory.CreatePolygon([
                new Coordinate(-180, top),
                new Coordinate(right, top),
                new Coordinate(right, bottom),
                new Coordinate(-180, bottom),
                new Coordinate(-180, top)
            ]);

            return GeometryFactory.BuildGeometry([box1, box2]);
        }
        else
        {
            var box = GeometryFactory.CreatePolygon([
                new Coordinate(left, top),
                new Coordinate(right, top),
                new Coordinate(right, bottom),
                new Coordinate(left, bottom),
                new Coordinate(left, top)
            ]);

            return GeometryFactory.BuildGeometry([box]);
        }
    }
}