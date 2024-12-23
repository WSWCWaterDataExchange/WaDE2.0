using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Responses;

namespace WesternStatesWater.WaDE.Contracts.Api;

public interface IWaterResourceManager
{
    Task<TResponse> Load<TRequest, TResponse>(TRequest request)
        where TRequest : WaterResourceLoadRequestBase
        where TResponse : WaterResourceLoadResponseBase;
}