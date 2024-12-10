using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Common.Extensions;

public static class AssemblyExtensions
{
    public static void RegisterRequestHandlers(this Assembly assembly, IServiceCollection serviceCollection)
    {
        var typeInfos = assembly.GetRequestHandlerTypeInfos();

        foreach (var implementationType in typeInfos)
        {
            var serviceType = implementationType.ImplementedInterfaces.First(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            serviceCollection.AddTransient(serviceType, implementationType);
        }
    }

    private static List<TypeInfo> GetRequestHandlerTypeInfos(this Assembly assembly)
    {
        var typeInfos = assembly.DefinedTypes
            .Where(type => type.ImplementedInterfaces
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        return typeInfos;
    }
}