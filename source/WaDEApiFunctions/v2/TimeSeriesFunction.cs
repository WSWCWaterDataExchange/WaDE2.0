using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

namespace WaDEApiFunctions.v2;

public class TimeSeriesFunction : FunctionBase
{
    private const string PathBase = "v2/collections/timeSeries/";
    private const string Tag = "Time Series";
    
    [Function(nameof(GetTimeSeriesCollectionMetadata))]
    [OpenApiOperation(operationId: "getTimeSeriesCollections", tags: [Tag], Summary = "Time Series collection metadata",
        Description = "TODO: description of the overlays collection.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public static async Task<HttpResponseData> GetTimeSeriesCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        return await CreateOkResponse(req, "Time Series collection description!");
    }   
}