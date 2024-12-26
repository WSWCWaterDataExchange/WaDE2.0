using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telerik.JustMock;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Engines.Contracts;
using WesternStatesWater.WaDE.Managers.Api.Handlers;

namespace WesternStatesWater.WaDE.Managers.Tests.WaterResourceManager;

public abstract class WaterResourceManagerTestsBase
{
    protected IWaterAllocationAccessor WaterAllocationAccessorMock { get; } =
        Mock.Create<IWaterAllocationAccessor>(Behavior.Strict);

    protected IFormattingEngine FormattingEngineMock { get; } =
        Mock.Create<IFormattingEngine>(Behavior.Strict);

    internal Api.WaterResourceManager CreateWaterResourceManager()
    {
        var logger = new ServiceCollection()
            .AddLogging(config => config.AddConsole())
            .BuildServiceProvider()
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<Api.WaterResourceManager>();

        return new Api.WaterResourceManager(
            Mock.Create<IManagerRequestHandlerResolver>(Behavior.Strict),
            FormattingEngineMock,
            WaterAllocationAccessorMock,
            logger
        );
    }
}