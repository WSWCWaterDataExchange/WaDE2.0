using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Common.Extensions;

namespace WesternStatesWater.WaDE.Managers.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterRequestHandlerResolver(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandlerResolver, RequestHandlerResolver>();

        Assembly.GetExecutingAssembly()
            .GetRequestHandlers()
            .ForEach(services.AddRequestHandler);

        return services;
    }
}