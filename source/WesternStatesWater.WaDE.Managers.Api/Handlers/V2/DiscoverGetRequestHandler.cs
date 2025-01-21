using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

public class DiscoverGetRequestHandler(IFormattingEngine formattingEngine) : IRequestHandler<DiscoveryMetadataGetRequest, DiscoveryMetadataGetResponse>
{
    public async Task<DiscoveryMetadataGetResponse> Handle(DiscoveryMetadataGetRequest request)
    {
        var discovery = new DiscoveryRequest();
        var discoveryFormatResponse = await formattingEngine.Format<DiscoveryRequest, DiscoveryResponse>(discovery);
        return discoveryFormatResponse.Map<DiscoveryMetadataGetResponse>();
    }
}