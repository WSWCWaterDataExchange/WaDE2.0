using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

internal class SiteFeatureSearchHandler : IRequestHandler<SiteFeatureSearchRequest, SiteFeatureSearchResponse>
{
    public Task<SiteFeatureSearchResponse> Handle(SiteFeatureSearchRequest request)
    {
        throw new System.NotImplementedException();
    }
}