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

public class OverlayFeatureItemGetRequestHandler(AccessorApi.IRegulatoryOverlayAccessor overlayAccessor, IFormattingEngine formattingEngine) : IRequestHandler<OverlayFeatureItemGetRequest, OverlayFeatureItemGetResponse>
{
    public async Task<OverlayFeatureItemGetResponse> Handle(OverlayFeatureItemGetRequest request)
    {
        var searchRequest = request.Map<OverlaySearchRequest>();
        var searchResponse = await overlayAccessor.Search<OverlaySearchRequest, OverlaySearchResponse>(searchRequest);
        
        var formatRequest = searchResponse.Map<OgcFeaturesFormattingRequest>();
        var formatResponse = await formattingEngine.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(formatRequest);
        return formatResponse.Map<OverlayFeatureItemGetResponse>();
    }
}