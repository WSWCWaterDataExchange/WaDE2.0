using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IAggregatedAmountsManager
    {
        Task<AggregatedAmountsSearchResponse> Load(AggregatedAmountsSearchRequest request);
    }
}