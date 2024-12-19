using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.Shared.Resolver;

namespace WesternStatesWater.WaDE.Engines;

public class EngineBase
{
    private readonly IRequestHandlerResolver _requestHandlerResolver;
    protected EngineBase(IRequestHandlerResolver requestHandlerResolver)
    {
        _requestHandlerResolver = requestHandlerResolver;
    }
    
    public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest request)
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        var response = await _requestHandlerResolver
            .Resolve<TRequest, TResponse>()
            .Handle(request);

        return response;
    }
}