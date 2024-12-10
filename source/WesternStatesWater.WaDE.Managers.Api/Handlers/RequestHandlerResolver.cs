using System;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers;

public class RequestHandlerResolver(IServiceProvider serviceProvider) : RequestHandlerResolverBase(serviceProvider)
{
    public override void ValidateTypeNamespace(Type requestType)
    {
        if (!requestType.Namespace!.Contains("Contracts.Api"))
        {
            throw new InvalidOperationException($"Type {requestType.FullName} is not a valid request type."
                                                + " Request types must be in the WesternStatesWater.WaDE.Contracts.Api namespace.");
        }
    }
}