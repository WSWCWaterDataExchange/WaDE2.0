namespace WesternStatesWater.WaDE.Engines.Contracts;

public interface IFormattingEngine
{
    Task<TResponse> Format<TRequest, TResponse>(TRequest request)
        where TRequest : FormattingRequestBase
        where TResponse : FormattingResponseBase;
}