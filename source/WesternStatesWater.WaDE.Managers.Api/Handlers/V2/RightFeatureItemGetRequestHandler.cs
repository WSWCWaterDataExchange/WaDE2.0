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

public class RightFeatureItemGetRequestHandler(
    AccessorApi.IWaterAllocationAccessor allocationAccessor,
    IFormattingEngine formattingEngine): IRequestHandler<RightFeatureItemGetRequest, RightFeatureItemGetResponse>
{
    public async Task<RightFeatureItemGetResponse> Handle(RightFeatureItemGetRequest request)
    {
        var searchRequest = request.Map<AllocationSearchRequest>();
        var searchResponse =
            await allocationAccessor.Search<AllocationSearchRequest, AllocationSearchResponse>(searchRequest);

        var formatRequest = searchResponse.Map<OgcFeaturesFormattingRequest>();
        var formatResponse = await formattingEngine.Format<OgcFeaturesFormattingRequest, OgcFeaturesFormattingResponse>(formatRequest);
        return formatResponse.Map<RightFeatureItemGetResponse>();
    }
}