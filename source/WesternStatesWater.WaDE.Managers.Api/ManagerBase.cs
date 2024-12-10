using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Managers.Api;

internal abstract class ManagerBase
{
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    protected ManagerBase(IRequestHandlerResolver requestHandlerResolver)
    {
        _requestHandlerResolver = requestHandlerResolver;
    }

    public async Task<ResponseBase> ExecuteAsync<TRequest>(TRequest request) where TRequest : RequestBase
    {
        var response = await _requestHandlerResolver
            .Resolve<TRequest>()
            .Handle(request);

        return response;
    }
}