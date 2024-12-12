using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Common.Extensions;

namespace WesternStatesWater.WaDE.Accessors.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
    {
        Assembly.GetExecutingAssembly().RegisterRequestHandlers(services);

        return services;
    }
}