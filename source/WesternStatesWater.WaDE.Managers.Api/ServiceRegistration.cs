using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WesternStatesWater.WaDE.Managers.Api;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterHandlerFactory(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandlerResolver, RequestHandlerResolver>();

        Assembly.GetExecutingAssembly().DefinedTypes
            .Where(type => type.ImplementedInterfaces.Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
            .ToList()
            .ForEach(implementationType =>
            {
                var serviceType = implementationType.ImplementedInterfaces.First(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>));

                services.AddTransient(serviceType, implementationType);
            });

        return services;
    }
}