using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Engines.Handlers;

namespace WesternStatesWater.WaDE.Engines;

internal class FormattingEngine : EngineBase, IFormattingEngine
{
    public FormattingEngine(IEngineRequestHandlerResolver requestHandlerResolver) : base(requestHandlerResolver)
    {
    }

    async Task<TResponse> IFormattingEngine.Format<TRequest, TResponse>(TRequest request)
    {
        return await ExecuteAsync<TRequest, TResponse>(request);
    }
}