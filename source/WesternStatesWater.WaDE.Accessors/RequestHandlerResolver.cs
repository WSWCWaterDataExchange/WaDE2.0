using System;
using WesternStatesWater.Shared.Resolver;

namespace WesternStatesWater.WaDE.Accessors;

public class RequestHandlerResolver(IServiceProvider serviceProvider) : RequestHandlerResolverBase(serviceProvider)
{
    public override void ValidateTypeNamespace(Type requestType, Type responseType)
    {
        if (!requestType.Namespace!.Contains("Requests"))
        {
            throw new InvalidOperationException($"Type {requestType.FullName} is not a valid request type."
                                                + " Request types must be in the WesternStatesWater.WaDE.Contracts.Api namespace.");
        }

        if (!responseType.Namespace!.Contains("Responses"))
        {
            throw new InvalidOperationException($"Type {responseType.FullName} is not a valid response type."
                                                + " Response types must be in the WesternStatesWater.WaDE.Contracts.Api namespace.");
        }
    }
}