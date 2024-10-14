using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace WaDEApiFunctions;

public abstract class FunctionBase
{
    protected async Task<HttpResponseData> CreateOkResponse<T>(
        HttpRequestData request,
        T response)
    {
        var data = request.CreateResponse(HttpStatusCode.OK);

        await data.WriteAsJsonAsync((object)response, new JsonObjectSerializer());

        return data;
    }

    protected Task<HttpResponseData> CreateBadRequestResponse(HttpRequestData request, ValidationError error)
    {
        var details = new HttpValidationProblemDetails(error.Errors)
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        return CreateProblemDetailsResponse(request, details, HttpStatusCode.BadRequest);
    }

    private async Task<HttpResponseData> CreateProblemDetailsResponse(
        HttpRequestData request,
        ProblemDetails details,
        HttpStatusCode statusCode)
    {
        var response = request.CreateResponse();

        // Casting to object for polymorphic serialization
        await response.WriteAsJsonAsync<object>(
            details,
            new JsonObjectSerializer(),
            statusCode);

        return response;
    }

    protected async Task<T> Deserialize<T>(HttpRequestData request) where T : class
    {
        return await JsonSerializer.DeserializeAsync<T>(request.Body);
    }
}