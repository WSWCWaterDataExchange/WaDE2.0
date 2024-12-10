using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Common.Extensions;

namespace WesternStatesWater.WaDE.Managers.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
    {
        Assembly.GetExecutingAssembly()
            .GetRequestHandlers()
            .ForEach(services.AddRequestHandler);

        return services;
    }
}