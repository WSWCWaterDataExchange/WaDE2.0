using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WesternStatesWater.Shared.Errors;
using WesternStatesWater.Shared.Exceptions;
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

public class RightFeatureItemGetRequestHandler(
    AccessorApi.IWaterAllocationAccessor allocationAccessor,
    IFormattingEngine formattingEngine,
    ILogger<RightFeatureItemGetRequestHandler> logger): IRequestHandler<RightFeatureItemGetRequest, RightFeatureItemGetResponse>
{
    public async Task<RightFeatureItemGetResponse> Handle(RightFeatureItemGetRequest request)
    {
        var searchRequest = request.Map<AllocationSearchRequest>();
        var searchResponse =
            await allocationAccessor.Search<AllocationSearchRequest, AllocationSearchResponse>(searchRequest);

        if (searchResponse.Allocations.Count != 1)
        {
            logger.LogInformation("{HandlerName} found {Count} sites for request {RequestedId}, but was expecting 1. Returning NotFoundError.", nameof(RightFeatureItemGetRequestHandler), searchResponse.Allocations.Count, request.Id);
            return new RightFeatureItemGetResponse
            {
                Error = new NotFoundError
                {
                    PublicMessage = "Water right not found.",

                }
            };
        }
        
        var formatRequest = searchResponse.Map<OgcFeaturesFormattingRequest>();
        var formatResponse = await formattingEngine.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(formatRequest);
        return formatResponse.Map<RightFeatureItemGetResponse>();
    }
}