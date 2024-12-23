using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Managers.Api;

internal partial class WaterResourceManager : ManagerApi.ISiteManager
{
    public async Task<TResponse> Search<TRequest, TResponse>(TRequest request)
        where TRequest : FeaturesSearchRequestBase where TResponse : FeaturesSearchResponseBase
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}