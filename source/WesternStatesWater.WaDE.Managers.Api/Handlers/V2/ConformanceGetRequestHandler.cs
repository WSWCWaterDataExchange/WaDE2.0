using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

public class ConformanceGetRequestHandler(IFormattingEngine formattingEngine) : IRequestHandler<ConformanceMetadataGetRequest, ConformanceMetadataGetResponse>
{
    public async Task<ConformanceMetadataGetResponse> Handle(ConformanceMetadataGetRequest request)
    {
        var conformanceReqeust = new ConformanceRequest();
        var conformanceResponse = await formattingEngine.Format<ConformanceRequest, ConformanceResponse>(conformanceReqeust);
        return conformanceResponse.Map<ConformanceMetadataGetResponse>();
    }
}