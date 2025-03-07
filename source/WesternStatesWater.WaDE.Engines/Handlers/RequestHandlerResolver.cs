using WesternStatesWater.Shared.Resolver;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public class RequestHandlerResolver(IServiceProvider serviceProvider)
    : RequestHandlerResolverBase(serviceProvider), IEngineRequestHandlerResolver
{
    public override void ValidateTypeNamespace(Type requestType, Type responseType)
    {
        if (!requestType.Namespace!.Contains("Requests"))
        {
            throw new InvalidOperationException($"Type {requestType.FullName} is not a valid request type."
                                                + " Request types must be in the WesternStatesWater.WaDE.Engines.Contracts namespace.");
        }

        if (!responseType.Namespace!.Contains("Responses"))
        {
            throw new InvalidOperationException($"Type {responseType.FullName} is not a valid response type."
                                                + " Response types must be in the WesternStatesWater.WaDE.Engines.Contracts namespace.");
        }
    }
}