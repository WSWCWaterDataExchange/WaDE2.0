using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

public class CollectionMetadataLoadHandler(IFormattingEngine formattingEngine) : IRequestHandler<CollectionMetadataRequest, CollectionMetadataResponse>
{
    public async Task<CollectionMetadataResponse> Handle(CollectionMetadataRequest request)
    {
        var dtoRequest = DtoMapper.Map<CollectionRequest>(request);
        var dtoResponse = await formattingEngine.Format<CollectionRequest, CollectionResponse>(dtoRequest);
        return DtoMapper.Map<CollectionMetadataResponse>(dtoResponse);
    }
}