using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using WaDEApiFunctions.Extensions;
using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.Shared.Errors;

namespace WaDEApiFunctions;

public abstract class FunctionBase
{
    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
            { new JsonStringEnumConverter(), new NetTopologySuite.IO.Converters.GeoJsonConverterFactory(false, "id") }
    };
    
    protected async Task<HttpResponseData> CheckUnmatchedParameters<T>(HttpRequestData req) where T : RequestBase
    {
        var (hasUnmatched, unmatchedParams) = req.Query.ContainsUnmatchedParameters<T>();
        if (hasUnmatched)
        {
            return await CreateErrorResponse(req,
                new ValidationError("query",
                    unmatchedParams.Select(param => $"Unknown query parameter: {param}").ToArray()));
        }
        return null;
    }
    
    protected async Task<HttpResponseData> CreateResponse(HttpRequestData request, ResponseBase response)
    {
        return response switch
        {
            { Error: null } => await CreateOkResponse(request, response),
            _ => await CreateErrorResponse(request, response.Error),
        };
    }

    /// <summary>
    /// Creates an HTTP response based on the provided response object and a property selector function.
    /// </summary>
    /// <param name="request">The HTTP request data.</param>
    /// <param name="response">The response object containing the data or error information.</param>
    /// <param name="propertySelector">A function to select a specific property from the response object.</param>
    /// <typeparam name="T">The type of the response object.</typeparam>
    /// <typeparam name="TResult">The type of the selected property.</typeparam>
    /// <returns>An HTTP response with the selected property or an error response.</returns>
    protected async Task<HttpResponseData> CreateResponse<T, TResult>(HttpRequestData request, T response,  Func<T, TResult> propertySelector) where T : ResponseBase
    {
        return response switch
        {
            { Error: null } => await CreateOkResponse(request, propertySelector(response)),
            _ => await CreateErrorResponse(request, response.Error),
        };
    }

    protected static async Task<HttpResponseData> CreateOkResponse<T>(
        HttpRequestData request,
        T response)
    {
        var data = request.CreateResponse(HttpStatusCode.OK);

        await data.WriteAsJsonAsync((object) response, new JsonObjectSerializer(JsonSerializerOptions));

        return data;
    }

    protected async Task<HttpResponseData> CreateErrorResponse(HttpRequestData request, ErrorBase error)
    {
        return error switch
        {
            InternalError => await CreateInternalServerErrorResponse(request),
            NotFoundError err => await CreateNotFoundResponse(request, err),
            ValidationError err => await CreateBadRequestResponse(request, err),
            _ => await CreateInternalServerErrorResponse(request)
        };
    }

    private Task<HttpResponseData> CreateInternalServerErrorResponse(HttpRequestData request)
    {
        var details = new ProblemDetails
        {
            Status = (int) HttpStatusCode.InternalServerError,
            Title = "An unexpected error has occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        return CreateProblemDetailsResponse(request, details, HttpStatusCode.InternalServerError);
    }

    private static Task<HttpResponseData> CreateNotFoundResponse(HttpRequestData request, NotFoundError err)
    {
        var details = new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = "Resource not found",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Detail = err.PublicMessage
        };

        return CreateProblemDetailsResponse(request, details, HttpStatusCode.NotFound);
    }
    
    private static Task<HttpResponseData> CreateBadRequestResponse(HttpRequestData request, ValidationError error)
    {
        var details = new HttpValidationProblemDetails(error.Errors)
        {
            Status = (int) HttpStatusCode.BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        return CreateProblemDetailsResponse(request, details, HttpStatusCode.BadRequest);
    }

    private static async Task<HttpResponseData> CreateProblemDetailsResponse(
        HttpRequestData request,
        ProblemDetails details,
        HttpStatusCode statusCode)
    {
        var response = request.CreateResponse();

        // Casting to object for polymorphic serialization
        await response.WriteAsJsonAsync<object>(
            details,
            new JsonObjectSerializer(JsonSerializerOptions),
            statusCode);

        return response;
    }

    protected static async Task<T> Deserialize<T>(HttpRequestData request, ILogger logger) where T : class
    {
        T result = null;

        try
        {
            // Workaround since every function currently calls deserialize regardless of http method type.
            result = request.Method != "GET"
                ? await JsonSerializer.DeserializeAsync<T>(request.Body, JsonSerializerOptions)
                : null;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize type '{TypeName}'", typeof(T).Name);
        }

        // For legacy reasons, return null instead of an error response. We should change this to return an error response.
        return result;
    }
}