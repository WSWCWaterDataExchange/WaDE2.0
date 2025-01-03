using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

internal class SiteFeaturesSearchRequestHandler(IFormattingEngine formattingEngine) : IRequestHandler<SiteFeaturesSearchRequest, SiteFeaturesSearchResponse>
{
    public async Task<SiteFeaturesSearchResponse> Handle(SiteFeaturesSearchRequest request)
    {
        var dtoRequest = request.Map<SiteFeaturesRequest>();
        
        if (dtoRequest.Limit <= 0)
            dtoRequest.Limit = 1000;
        
        var dtoResponse = await formattingEngine.Format<SiteFeaturesRequest, SiteFeaturesResponse>(dtoRequest);
        return dtoResponse.Map<SiteFeaturesSearchResponse>();
    }
}