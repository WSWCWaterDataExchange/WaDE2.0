using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Managers.Api.Extensions;

namespace WesternStatesWater.WaDE.Managers.Api;

internal abstract class ManagerBase
{
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    protected ManagerBase(IRequestHandlerResolver requestHandlerResolver)
    {
        _requestHandlerResolver = requestHandlerResolver;
    }

    public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest request)
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        var validationResult = await request.ValidateAsync();
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult);
        }

        var response = await _requestHandlerResolver
            .Resolve<TRequest, TResponse>()
            .Handle(request);

        return response;
    }
}