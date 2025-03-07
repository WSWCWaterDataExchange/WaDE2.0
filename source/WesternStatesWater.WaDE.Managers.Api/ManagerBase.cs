using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.Shared.Errors;
using WesternStatesWater.Shared.Exceptions;
using WesternStatesWater.Shared.Extensions;
using WesternStatesWater.Shared.Resolver;

namespace WesternStatesWater.WaDE.Managers.Api;

internal abstract class ManagerBase
{
    private readonly ILogger _logger;

    private readonly IRequestHandlerResolver _requestHandlerResolver;

    protected ManagerBase(ILogger logger, IRequestHandlerResolver requestHandlerResolver)
    {
        _logger = logger;
        _requestHandlerResolver = requestHandlerResolver;
    }

    public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest request)
        where TRequest : RequestBase
        where TResponse : ResponseBase
    {
        try
        {
            var validationResult = await request.ValidateAsync();

            if (!validationResult.IsValid)
            {
                return CreateErrorResponse<TResponse>(new ValidationError(validationResult.ToDictionary()));
            }

            var response = await _requestHandlerResolver
                .Resolve<TRequest, TResponse>()
                .Handle(request);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request");

            return CreateErrorResponse<TResponse>(new InternalError());
        }
    }

    private TResponse CreateErrorResponse<TResponse>(ErrorBase error) where TResponse : ResponseBase
    {
        try
        {
            var response = (TResponse)Activator.CreateInstance(typeof(TResponse))!;
            response.Error = error;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create error response for type {ResponseTypeName} with error type {ErrorTypeName}",
                typeof(TResponse).FullName,
                error.GetType().FullName
            );

            return (TResponse)new ResponseBase { Error = new InternalError() };
        }
    }
}