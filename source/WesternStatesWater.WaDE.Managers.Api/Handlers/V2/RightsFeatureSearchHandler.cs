using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

public class RightsFeatureSearchHandler : IRequestHandler<RightsFeatureSearchRequest, RightsFeatureSearchResponse>
{
    public Task<RightsFeatureSearchResponse> Handle(RightsFeatureSearchRequest request)
    {
        throw new System.NotImplementedException();
    }
}