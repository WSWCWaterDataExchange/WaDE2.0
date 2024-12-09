using System;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

namespace WesternStatesWater.WaDE.Managers.Api;

public interface IRequestHandlerResolver
{
    IRequestHandler<T> Resolve<T>() where T : RequestBase;
}

internal class RequestHandlerResolver : IRequestHandlerResolver
{
    private readonly IServiceProvider _serviceProvider;

    public RequestHandlerResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRequestHandler<T> Resolve<T>() where T : RequestBase
    {
        var requestType = typeof(T);

        if (requestType.Namespace is null || !requestType.Namespace.Contains("Contracts.Api"))
        {
            throw new InvalidOperationException($"Type {requestType.FullName} is not a valid request type."
                                                + " Request types must be in the WesternStatesWater.WaDE.Managers namespace.");
        }

        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = (IRequestHandler<T>)_serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.FullName}.");
        }

        return handler;
    }
}

public interface IRequestHandler<in T> where T : RequestBase
{
    Task<ResponseBase> Handle(T request);
}

internal class SearchOverlayRequestHandler : IRequestHandler<SearchOverlayRequest>
{
    public Task<ResponseBase> Handle(SearchOverlayRequest request)
    {
        throw new NotImplementedException("what up doe");
    }
}