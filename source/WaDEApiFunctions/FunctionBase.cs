using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
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