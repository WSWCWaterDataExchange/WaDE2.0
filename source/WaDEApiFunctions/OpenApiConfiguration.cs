using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WaDEApiFunctions;

public static class OpenApiConfiguration
{
    public static void AddOpenApi(this IServiceCollection services)
    {
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
    }
}