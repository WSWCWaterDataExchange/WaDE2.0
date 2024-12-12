using System;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WaDEApiFunctions;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Engines;
using WesternStatesWater.WaDE.Managers.Api;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using EngineApi = WesternStatesWater.WaDE.Engines.Contracts;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;
using ManagerExt = WesternStatesWater.WaDE.Managers.Api.Extensions;
using AccessorExt = WesternStatesWater.WaDE.Accessors.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.UseNewtonsoftJson();
        builder.UseMiddleware<HttpContextAccessorMiddleware>();
    })
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

        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                Info = new OpenApiInfo()
                {
                    Version = DefaultOpenApiConfigurationOptions.GetOpenApiDocVersion(),
                    Title = DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle()
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = DefaultOpenApiConfigurationOptions.GetOpenApiVersion(),
                IncludeRequestingHostName =
                    DefaultOpenApiConfigurationOptions.IsFunctionsRuntimeEnvironmentDevelopment(),
                ForceHttps = DefaultOpenApiConfigurationOptions.IsHttpsForced(),
                ForceHttp = DefaultOpenApiConfigurationOptions.IsHttpForced(),
            };

            return options;
        });

        services.AddHttpContextAccessor();

        services.AddSingleton(configuration);

        services
            .AddScoped<IRequestHandlerResolver, WesternStatesWater.WaDE.Managers.Api.Handlers.RequestHandlerResolver>();
        services.AddScoped<IRequestHandlerResolver, WesternStatesWater.WaDE.Accessors.RequestHandlerResolver>();
        ManagerExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);
        AccessorExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);

        services.AddTransient<ManagerApi.IAggregatedAmountsManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IRegulatoryOverlayManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.ISiteVariableAmountsManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IWaterAllocationManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IMetadataManager, WaterResourceManager>();

        services.AddTransient<EngineApi.IValidationEngine, ValidationEngine>();

        services.AddTransient<AccessorApi.IAggregatedAmountsAccessor, AggregratedAmountsAccessor>();
        services.AddTransient<AccessorApi.IRegulatoryOverlayAccessor, RegulatoryOverlayAccessor>();
        services.AddTransient<AccessorApi.ISiteVariableAmountsAccessor, SiteVariableAmountsAccessor>();
        services.AddTransient<AccessorApi.IWaterAllocationAccessor, WaterAllocationAccessor>();
        services
            .AddTransient<WesternStatesWater.WaDE.Accessors.Sites.ISiteAccessor,
                WesternStatesWater.WaDE.Accessors.Sites.SiteAccessor>();
    })
    .Build();

await host.RunAsync();