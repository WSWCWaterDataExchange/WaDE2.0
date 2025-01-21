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
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Engines;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Managers.Api;
using WesternStatesWater.WaDE.Managers.Api.Handlers;
using WesternStatesWater.WaDE.Utilities;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using AccessorExt = WesternStatesWater.WaDE.Accessors.Extensions;
using EngineApi = WesternStatesWater.WaDE.Engines.Contracts;
using EngineExt = WesternStatesWater.WaDE.Engines.Extensions;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;
using ManagerExt = WesternStatesWater.WaDE.Managers.Api.Extensions;

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
                    Title = DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle(),
                    Description = DefaultOpenApiConfigurationOptions.GetOpenApiDocDescription()
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

        services.AddScoped<
            IManagerRequestHandlerResolver,
            WesternStatesWater.WaDE.Managers.Api.Handlers.RequestHandlerResolver
        >();

        services.AddScoped<
            IEngineRequestHandlerResolver,
            WesternStatesWater.WaDE.Engines.Handlers.RequestHandlerResolver
        >();

        services.AddScoped<
            IAccessorRequestHandlerResolver,
            WesternStatesWater.WaDE.Accessors.Handlers.RequestHandlerResolver
        >();
        
        AccessorExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);
        ManagerExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);
        EngineExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);
        
        services.AddTransient<ManagerApi.IMetadataManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IWaterResourceManager, WaterResourceManager>();

        services.AddTransient<EngineApi.IValidationEngine, ValidationEngine>();
        services.AddTransient<EngineApi.IFormattingEngine, FormattingEngine>();

        services.AddTransient<AccessorApi.IAggregatedAmountsAccessor, AggregratedAmountsAccessor>();
        services.AddTransient<AccessorApi.IRegulatoryOverlayAccessor, RegulatoryOverlayAccessor>();
        services.AddTransient<AccessorApi.ISiteVariableAmountsAccessor, SiteVariableAmountsAccessor>();
        services.AddTransient<AccessorApi.IWaterAllocationAccessor, WaterAllocationAccessor>();
        services.AddTransient<AccessorApi.ISiteAccessor, SiteAccessor>();

        services.AddTransient<IContextUtility, ContextUtility>();
    })
    .Build();

await host.RunAsync();