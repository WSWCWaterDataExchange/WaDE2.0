using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Engines.Contracts;

namespace WesternStatesWater.WaDE.Engines;

public class FormattingEngine : EngineBase, IFormattingEngine
{
    protected FormattingEngine(IRequestHandlerResolver requestHandlerResolver) : base(requestHandlerResolver)
    {
    }

    public async Task<TResponse> Format<TRequest, TResponse>(TRequest request) where TRequest : FormattingRequestBase
        where TResponse : FormattingResponseBase
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}