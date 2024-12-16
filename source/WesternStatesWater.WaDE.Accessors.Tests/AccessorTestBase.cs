using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Common.Extensions;
using WesternStatesWater.WaDE.Tests.Helpers;

namespace WesternStatesWater.WaDE.Accessors.Tests;

public class AccessorTestBase : DbTestBase
{
    protected IServiceProvider ServiceProvider { get; private set; }

    [TestInitialize]
    public void AccessorTestBaseInitialize()
    {
        IConfiguration config = Configuration.GetConfiguration();

        var services = new ServiceCollection()
            .AddScoped(_ => config)
            .AddScoped<IRequestHandlerResolver, RequestHandlerResolver>()
            .AddScoped<ISiteAccessor, SiteAccessor>();

        typeof(AccessorBase).Assembly.RegisterRequestHandlers(services);

        ServiceProvider = services.BuildServiceProvider();
    }
}