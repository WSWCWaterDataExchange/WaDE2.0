using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;

namespace WesternStatesWater.WaDE.Engines.Handlers;

internal class OverlayFeaturesRequestHandler(
    IConfiguration configuration,
    IRegulatoryOverlayAccessor regulatoryOverlayAccessor
) : OgcFormattingHandlerBase(configuration),
    IRequestHandler<OverlayFeaturesRequest, OverlayFeaturesResponse>
{
    public async Task<OverlayFeaturesResponse> Handle(OverlayFeaturesRequest request)
    {
        var searchRequest = new OverlaySearchRequest
        {
            FilterBoundary = request.Bbox != null ? ConvertBoundaryBoxToPolygon(request.Bbox) : null,
            LastKey = request.LastOverlayUuid,
            Limit = request.Limit,
            OverlayUuids = request.OverlayUuids,
            SiteUuids = request.SiteUuids
        };
        var searchResponse =
            await regulatoryOverlayAccessor.Search<OverlaySearchRequest, OverlaySearchResponse>(searchRequest);
        
        List<Feature> features = [];
        features.AddRange(searchResponse.Overlays.Select(overlay => new Feature
        {
            Geometry = overlay.Areas,
            Attributes = new AttributesTable
            {
                {"id", overlay.OverlayUuid},
                {"nId", overlay.OverlayNativeId},
                {"name", overlay.RegulatoryName},
                {"desc", overlay.RegulatoryDescription},
                {"status", overlay.RegulatoryStatus},
                {"agency", overlay.OversightAgency},
                {"statute", overlay.RegulatoryStatute},
                {"statueRef", overlay.RegulatoryStatuteLink},
                {"statueEffDate", overlay.StatutoryEffectiveDate},
                {"statueEndDate", overlay.StatutoryEndDate},
                {"oType", overlay.OverlayType},
                {"wSource", overlay.WaterSource},
                {"areaNames", overlay.AreaNames},
                {"areaNIds", overlay.AreaNativeIds},
                {"sites", overlay.SiteUuids}
            }
        }));

        var links = new LinkBuilder(ServerUrl, ApiPath)
            .AddLandingPage();

        if (searchResponse.Overlays.Count > 0)
        {
            links.AddNextFeatures(Constants.OverlaysCollectionId, searchResponse.Overlays[^1].OverlayUuid);
        }

        return new OverlayFeaturesResponse
        {
            Features = features.ToArray(),
            Links = links.Build()
        };
    }
}