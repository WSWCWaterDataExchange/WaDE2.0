using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors;

public class AccessorBase
{
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    protected AccessorBase(IRequestHandlerResolver requestHandlerResolver)
    {
        _requestHandlerResolver = requestHandlerResolver;
    }
    
    protected async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest request)
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        var response = await _requestHandlerResolver
            .Resolve<TRequest, TResponse>()
            .Handle(request);

        return response;
    }
}