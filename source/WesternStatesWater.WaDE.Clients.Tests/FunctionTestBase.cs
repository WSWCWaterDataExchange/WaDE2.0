using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WesternStatesWater.WaDE.Clients.Tests;

public abstract class FunctionTestBase
{
    private readonly ILoggerFactory _loggerFactory;

    protected FunctionTestBase()
    {
        _loggerFactory = new ServiceCollection()
            .AddLogging(config => config.AddConsole())
            .BuildServiceProvider()
            .GetRequiredService<ILoggerFactory>();
    }

    protected ILogger<T> CreateLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }
}