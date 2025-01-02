using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WaDE.Managers.Api.Handlers;

namespace WesternStatesWater.WaDE.Managers.Api;

internal class WaterResourceManager(
    IManagerRequestHandlerResolver requestHandlerResolver,
    ILogger<WaterResourceManager> logger
) : ManagerBase(logger, requestHandlerResolver), ManagerApi.IMetadataManager, ManagerApi.IWaterResourceManager
{
    async Task<TResponse> ManagerApi.IMetadataManager.Load<TRequest, TResponse>(TRequest request)
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }

    async Task<TResponse> ManagerApi.IWaterResourceManager.Load<TRequest, TResponse>(TRequest request)
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}