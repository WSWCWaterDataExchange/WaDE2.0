namespace WesternStatesWater.WaDE.Common.Contracts;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : RequestBase
    where TResponse : ResponseBase
{
    Task<TResponse> Handle(TRequest request);
}