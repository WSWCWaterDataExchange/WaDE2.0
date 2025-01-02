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
        request.Limit ??= 1000;

        var dtoRequest = request.Map<SiteFeaturesRequest>();
        var dtoResponse = await formattingEngine.Format<SiteFeaturesRequest, SiteFeaturesResponse>(dtoRequest);
        return dtoResponse.Map<SiteFeaturesSearchResponse>();
    }
}