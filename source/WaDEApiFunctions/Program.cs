using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WaDEApiFunctions;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Managers.Api;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder => { builder.UseMiddleware<HttpContextAccessorMiddleware>(); })
    .ConfigureAppConfiguration((_, configBuilder) =>
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        configBuilder
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"settings.{environment}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"settings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddHttpContextAccessor();

        services.AddSingleton(configuration);

        services.AddTransient<ManagerApi.IWaterAllocationManager, WaterResourceManager>();
        services.AddTransient<AccessorApi.IWaterAllocationAccessor, WaterAllocationAccessor>();

        services.AddTransient<ManagerApi.IAggregatedAmountsManager, WaterResourceManager>();
        services.AddTransient<AccessorApi.IAggregatedAmountsAccessor, AggregratedAmountsAccessor>();

        services.AddTransient<ManagerApi.ISiteVariableAmountsManager, WaterResourceManager>();
        services.AddTransient<AccessorApi.ISiteVariableAmountsAccessor, SiteVariableAmountsAccessor>();

        services.AddTransient<ManagerApi.IRegulatoryOverlayManager, WaterResourceManager>();
        services.AddTransient<AccessorApi.IRegulatoryOverlayAccessor, RegulatoryOverlayAccessor>();
    })
    .Build();

host.Run();