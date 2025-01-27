using System.Reflection;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;

namespace WesternStatesWater.WaDE.Integration.Tests.Function;

public class FunctionTestBase
{
    public List<MethodInfo> FunctionHttpTriggers()
    {
        Assembly assembly = typeof(WaDEApiFunctions.OpenApiConfiguration).GetTypeInfo()
            .Assembly;

        var @namespace = "WaDEApiFunctions.v2";
        var functionClasses = assembly.GetTypes()
            .Where(t =>
                t.FullName != null
                && String.Equals(t.Namespace, @namespace, StringComparison.Ordinal))
            .ToArray();

        functionClasses.Should().NotBeEmpty();


        var httpTriggerMethods = functionClasses.SelectMany(functionClass => functionClass.GetMethods().Where(info =>
                info.IsPublic &&
                info.GetCustomAttributes().Any(attr => attr is FunctionAttribute) &&
                info.GetParameters().Any(
                    paramInfo => paramInfo.GetCustomAttributes().Any(attr =>
                        attr is HttpTriggerAttribute))))
            .ToList();

        return httpTriggerMethods;
    }
}