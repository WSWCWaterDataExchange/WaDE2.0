using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class OverlaysFunction(IMetadataManager metadataManager, IWaterResourceManager waterResourceManager) : FunctionBase
{
    private const string PathBase = "v2/collections/overlays/";
    private const string Tag = "Overlays";

    [Function(nameof(GetOverlaysCollectionMetadata))]
    [OpenApiOperation(operationId: "getOverlaysCollections", tags: [Tag], Summary = "Overlays collection metadata",
        Description = "WaDE overlays collection.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "Successful request", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> GetOverlaysCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest
        {
            CollectionId = Constants.OverlaysCollectionId
        };
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateOkResponse(req, response.Collection);
    }

    [Function(nameof(GetOverlays))]
    [OpenApiOperation(operationId: "getOverlays", tags: [Tag], Summary = "Get overlay collection items",
        Description = "List of overlays.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter("limit", Type = typeof(int), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "The maximum number of items to return.")]
    [OpenApiParameter("bbox", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Bounding box to filter results.")]
    [OpenApiParameter("next", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Next page")]
    [OpenApiParameter("siteIds", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Comma separated list of site ids")]
    [OpenApiParameter("overlayIds", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Comma separated list of overlay ids")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> GetOverlays(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new OverlayFeaturesSearchRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            OverlayUuids = req.Query["overlayIds"],
            SiteUuids = req.Query["siteIds"]
        };
        var response =
            await waterResourceManager.Search<OverlayFeaturesSearchRequest, OverlayFeaturesSearchResponse>(request);

        return await CreateOkResponse(req, response);
    }
}