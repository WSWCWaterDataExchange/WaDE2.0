using System.Reflection;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Common.Extensions;

public static class AssemblyExtensions
{
    public static List<TypeInfo> GetRequestHandlers(this Assembly assembly)
    {
        var infos = assembly.DefinedTypes
            .Where(type => type.ImplementedInterfaces
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
            .ToList();

        return infos;
    }
}