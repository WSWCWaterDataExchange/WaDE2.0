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

public class TimeSeriesFeatureItemGetRequestHandler(
    AccessorApi.ISiteVariableAmountsAccessor siteVariableAmountsAccessor,
    IFormattingEngine formattingEngine,
    ILogger<TimeSeriesFeatureItemGetRequestHandler> logger)
    : IRequestHandler<TimeSeriesFeatureItemGetRequest, TimeSeriesFeatureItemGetResponse>
{
    public async Task<TimeSeriesFeatureItemGetResponse> Handle(TimeSeriesFeatureItemGetRequest request)
    {
        var searchRequest = request.Map<TimeSeriesSearchRequest>();
        var searchResponse =
            await siteVariableAmountsAccessor.Search<TimeSeriesSearchRequest, TimeSeriesSearchResponse>(searchRequest);

        if (searchResponse.Sites.Count != 1)
        {
            logger.LogInformation(
                "{HandlerName} found {Count} sites for request {RequestedId}, but was expecting 1. Returning NotFoundError.",
                nameof(SiteFeatureItemGetRequestHandler), searchResponse.Sites.Count, request.Id);
            return new TimeSeriesFeatureItemGetResponse
            {
                Error = new NotFoundError
                {
                    PublicMessage = "Site not found.",
                }
            };
        }

        var formatRequest = searchResponse.Map<OgcFeaturesFormattingRequest>();
        var formatResponse =
            await formattingEngine.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(formatRequest);
        return formatResponse.Map<TimeSeriesFeatureItemGetResponse>();
    }
}