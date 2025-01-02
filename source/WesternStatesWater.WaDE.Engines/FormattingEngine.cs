using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Handlers;

namespace WesternStatesWater.WaDE.Engines;

internal class FormattingEngine : EngineBase, IFormattingEngine
{
    public FormattingEngine(IEngineRequestHandlerResolver requestHandlerResolver) : base(requestHandlerResolver)
    {
    }

    public async Task<TResponse> Format<TRequest, TResponse>(TRequest request) where TRequest : FormattingRequestBase
        where TResponse : FormattingResponseBase
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}