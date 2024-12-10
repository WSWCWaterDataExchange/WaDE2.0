using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Common.Extensions;

namespace WesternStatesWater.WaDE.Managers.Api;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterHandlerFactory(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandlerResolver, RequestHandlerResolver>();

        Assembly.GetExecutingAssembly()
            .GetRequestHandlers()
            .ForEach(services.AddRequestHandler);

        return services;
    }
}