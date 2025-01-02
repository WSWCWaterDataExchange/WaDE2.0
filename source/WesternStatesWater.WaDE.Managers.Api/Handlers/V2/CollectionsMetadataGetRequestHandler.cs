using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

/// <summary>
/// Loads WaDEs Feature Collections following the OGC API - Features standard.
/// </summary>
public class CollectionsMetadataGetRequestHandler(IFormattingEngine formattingEngine) : IRequestHandler<CollectionsMetadataGetRequest, CollectionsMetadataGetResponse>
{
    public async Task<CollectionsMetadataGetResponse> Handle(CollectionsMetadataGetRequest getRequest)
    {
        var dtoRequest = new CollectionsRequest();
        var response = await formattingEngine.Format<CollectionsRequest, CollectionsResponse>(dtoRequest);
        return DtoMapper.Map<CollectionsMetadataGetResponse>(response);
    }
}