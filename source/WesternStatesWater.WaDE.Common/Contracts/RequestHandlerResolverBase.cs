namespace WesternStatesWater.WaDE.Common.Contracts;

public abstract class RequestHandlerResolverBase : IRequestHandlerResolver
{
    protected IServiceProvider ServiceProvider { get; }

    public RequestHandlerResolverBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public abstract void ValidateTypeNamespace(Type requestType);

    public IRequestHandler<TRequest, TResponse> Resolve<TRequest, TResponse>()
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        var requestType = typeof(TRequest);

        if (requestType.Namespace is null)
        {
            throw new InvalidOperationException($"Type {requestType.FullName} is not a valid request type."
                                                + " Request types must be in a namespace.");
        }

        ValidateTypeNamespace(requestType);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType);
        var handler = ServiceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.FullName}.");
        }

        return (IRequestHandler<TRequest, TResponse>)handler;
    }
}