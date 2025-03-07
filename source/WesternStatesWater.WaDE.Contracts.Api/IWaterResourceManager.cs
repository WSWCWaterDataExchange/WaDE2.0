using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Contracts.Api;

public interface IWaterResourceManager
{
    Task<TResponse> Load<TRequest, TResponse>(TRequest request)
        where TRequest : WaterResourceLoadRequestBase
        where TResponse : WaterResourceLoadResponseBase;
    
    public Task<TResponse> Search<TRequest, TResponse>(TRequest request)
        where TRequest : WaterResourceSearchRequestBase
        where TResponse : WaterResourceSearchResponseBase;
}