using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

namespace WesternStatesWater.WaDE.Managers.Api
{
    internal partial class WaterResourceManager : IAggregatedAmountsManager
    {
        async Task<AggregatedAmountsSearchResponse> IAggregatedAmountsManager.Load(
            AggregatedAmountsSearchRequest request
        )
        {
            return await ExecuteAsync<AggregatedAmountsSearchRequest, AggregatedAmountsSearchResponse>(request);
        }
    }
}