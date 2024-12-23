using System;
using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

public class FeaturesSearchHandler(IFormattingEngine formattingEngine) : IRequestHandler<FeaturesSearchRequestBase, FeaturesSearchResponseBase>
{
    public async Task<FeaturesSearchResponseBase> Handle(FeaturesSearchRequestBase request)
    {
        request.Limit ??= 1000;

        return request switch
        {
            SiteFeaturesSearchRequest siteSearchRequest => await FormatSites(siteSearchRequest),
            _ => throw new NotSupportedException($"Request type {request.GetType().Name} is not supported.")
        };
    }

    internal async Task<SiteFeaturesSearchResponse> FormatSites(SiteFeaturesSearchRequest request)
    {
        var dtoRequest = request.Map<SiteFeaturesRequest>();
        var dtoResponse = await formattingEngine.Format<FeaturesRequestBase, FeaturesResponseBase>(dtoRequest);
        return dtoResponse.Map<SiteFeaturesSearchResponse>();
    }
}