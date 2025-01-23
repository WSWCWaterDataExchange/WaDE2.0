using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WesternStatesWater.Shared.Errors;
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

public class OverlayFeatureItemGetRequestHandler(
    AccessorApi.IRegulatoryOverlayAccessor overlayAccessor, 
    IFormattingEngine formattingEngine, 
    ILogger<OverlayFeatureItemGetRequestHandler> logger) : IRequestHandler<OverlayFeatureItemGetRequest, OverlayFeatureItemGetResponse>
{
    public async Task<OverlayFeatureItemGetResponse> Handle(OverlayFeatureItemGetRequest request)
    {
        var searchRequest = request.Map<OverlaySearchRequest>();
        var searchResponse = await overlayAccessor.Search<OverlaySearchRequest, OverlaySearchResponse>(searchRequest);
        
        if (searchResponse.Overlays.Count != 1)
        {
            logger.LogInformation("{HandlerName} found {Count} sites for request {RequestedId}, but was expecting 1. Returning NotFoundError.", nameof(OverlayFeatureItemGetRequestHandler), searchResponse.Overlays.Count, request.Id);
            return new OverlayFeatureItemGetResponse
            {
                Error = new NotFoundError
                {
                    PublicMessage = "Overlay not found.",

                }
            };
        }
        
        var formatRequest = searchResponse.Map<OgcFeaturesFormattingRequest>();
        var formatResponse = await formattingEngine.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(formatRequest);
        return formatResponse.Map<OverlayFeatureItemGetResponse>();
    }
}