using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WaDEApiFunctions.v2;

public class OverlaysFunction : FunctionBase
{
    private const string PathBase = "v2/collections/overlays/";
    private const string Tag = "Overlays";
    
    [Function(nameof(GetOverlaysCollectionMetadata))]
    [OpenApiOperation(operationId: "getOverlaysCollections", tags: [Tag], Summary = "Overlays collection metadata",
        Description = "TODO: description of the overlays collection.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public static async Task<HttpResponseData> GetOverlaysCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        return await CreateOkResponse(req, "Overlays collection description!");
    }
}