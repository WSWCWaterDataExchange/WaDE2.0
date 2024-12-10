namespace WesternStatesWater.WaDE.Common.Contracts;

public class RequestHandlerResolver : IRequestHandlerResolver
{
    private readonly IServiceProvider _serviceProvider;

    public RequestHandlerResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRequestHandler<TRequest, TResponse> Resolve<TRequest, TResponse>()
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        var requestType = typeof(TRequest);

        if (requestType.Namespace is null || !requestType.Namespace.Contains("Contracts.Api"))
        {
            throw new InvalidOperationException($"Type {requestType.FullName} is not a valid request type."
                                                + " Request types must be in the WesternStatesWater.WaDE.Managers namespace.");
        }

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType);
        var handler = _serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.FullName}.");
        }

        return (IRequestHandler<TRequest, TResponse>)handler;
    }
}