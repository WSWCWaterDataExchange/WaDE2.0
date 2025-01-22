using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

public class SiteFeatureItemGetRequestHandler(AccessorApi.ISiteAccessor siteAccessor, IFormattingEngine formattingEngine) : IRequestHandler<SiteFeatureItemGetRequest, SiteFeatureItemGetResponse>
{
    public async Task<SiteFeatureItemGetResponse> Handle(SiteFeatureItemGetRequest request)
    {
        var searchRequest = request.Map<SiteSearchRequest>();
        var searchResponse = await siteAccessor.Search<SiteSearchRequest, SiteSearchResponse>(searchRequest);

        var formatRequest = searchResponse.Map<OgcFeaturesFormattingRequest>();
        var formatResponse = await formattingEngine.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(formatRequest);
        return formatResponse.Map<SiteFeatureItemGetResponse>();
    }
}