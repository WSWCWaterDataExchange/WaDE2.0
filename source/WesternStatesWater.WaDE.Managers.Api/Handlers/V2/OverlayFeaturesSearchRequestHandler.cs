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

public class OverlayFeaturesSearchRequestHandler(IFormattingEngine formattingEngine, AccessorApi.IRegulatoryOverlayAccessor regulatoryOverlayAccessor) : IRequestHandler<OverlayFeaturesSearchRequest, OverlayFeaturesSearchResponse>
{
    public async Task<OverlayFeaturesSearchResponse> Handle(OverlayFeaturesSearchRequest request)
    {
        var searchRequest = request.Map<OverlaySearchRequest>();
        
        // OGC API Spec - If the value of the limit parameter is larger than the maximum value, this SHALL NOT result in an error (instead use the maximum as the parameter value).
        // https://docs.ogc.org/is/17-069r4/17-069r4.html#_parameter_limit
        if (searchRequest.Limit is 0 or > 1_000)
            searchRequest.Limit = 1_000;
        
        var searchResponse =
            await regulatoryOverlayAccessor.Search<OverlaySearchRequest, OverlaySearchResponse>(searchRequest);

        // Map to engine?
        var formatRequest = searchResponse.Map<OgcFeaturesFormattingRequest>();
        var dtoResponse = await formattingEngine.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(formatRequest);
        return dtoResponse.Map<OverlayFeaturesSearchResponse>();
    }
}