using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Accessors.Sites;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Common.Extensions;
using WesternStatesWater.WaDE.Tests.Helpers;

namespace WesternStatesWater.WaDE.Accessors.Tests;

public class AccessorTestBase : DbTestBase
{
    public IServiceProvider ServiceProvider;

    [TestInitialize]
    public void AccessorTestBaseInitialize()
    {
        var config = (IConfiguration) Configuration.GetConfiguration();
        var services = new ServiceCollection()
            .AddScoped(_ => config)
            .AddScoped<IRequestHandlerResolver, RequestHandlerResolver>()
            .AddScoped<ISiteAccessor, SiteAccessor>();

        Assembly.GetExecutingAssembly().RegisterRequestHandlers(services);
        ServiceProvider = services.BuildServiceProvider();
    }
}