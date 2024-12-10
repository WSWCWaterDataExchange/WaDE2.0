using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddRequestHandler(this IServiceCollection services, TypeInfo implementationType)
    {
        var serviceType = implementationType.ImplementedInterfaces.First(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>));

        services.AddTransient(serviceType, implementationType);
    }
}