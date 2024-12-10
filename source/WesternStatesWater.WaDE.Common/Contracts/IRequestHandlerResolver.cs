namespace WesternStatesWater.WaDE.Common.Contracts;

public interface IRequestHandlerResolver
{
    IRequestHandler<TRequest, TResponse> Resolve<TRequest, TResponse>()
        where TRequest : RequestBase
        where TResponse : ResponseBase;
}