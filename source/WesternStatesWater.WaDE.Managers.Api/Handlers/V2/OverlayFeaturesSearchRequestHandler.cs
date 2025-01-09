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
        
        // When limit is 0 or over 10,000 default to 10,000.
        // OGC API Spec - If the value of the limit parameter is larger than the maximum value, this SHALL NOT result in an error (instead use the maximum as the parameter value).
        // https://docs.ogc.org/is/17-069r4/17-069r4.html#_parameter_limit
        if (searchRequest.Limit is 0 or > 10_000)
            searchRequest.Limit = 10_000;
        
        var searchResponse =
            await regulatoryOverlayAccessor.Search<OverlaySearchRequest, OverlaySearchResponse>(searchRequest);

        // Map to engine?
        var formatRequest = searchResponse.Map<FeaturesRequest>();
        var dtoResponse = await formattingEngine.Format<FeaturesRequest, FeaturesResponse>(formatRequest);
        return dtoResponse.Map<OverlayFeaturesSearchResponse>();
    }
}