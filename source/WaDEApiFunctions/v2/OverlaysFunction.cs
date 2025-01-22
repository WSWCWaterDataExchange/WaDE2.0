using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
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
        var request = new CollectionMetadataGetRequest();
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
    [OpenApiParameter("states", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Comma separated list of state abbreviations")]
    [OpenApiParameter("overlayTypes", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Comma separated list of overlay types")]
    [OpenApiParameter("waterSourceTypes", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Comma separated list of water source types")]
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
        var request = new OverlayFeaturesItemRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            OverlayUuids = req.Query["overlayIds"],
            SiteUuids = req.Query["siteIds"],
            States = req.Query["states"],
            OverlayTypes = req.Query["overlayTypes"],
            WaterSourceTypes = req.Query["waterSourceTypes"]
        };
        var response =
            await waterResourceManager.Search<OverlayFeaturesSearchRequestBase, OverlayFeaturesSearchResponse>(request);

        return await CreateOkResponse(req, response);
    }
    
    [Function(nameof(GetOverlaysInArea))]
    [OpenApiOperation(operationId: "getOverlaysInArea", tags: [Tag], Summary = "Get overlay collection items in the given area",
        Description = "List of overlays.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter("limit", Type = typeof(int), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "The maximum number of items to return.")]
    [OpenApiParameter("coords", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Only data that has a geometry that intersects the area defined by the polygon are selected.\n\nThe polygon is defined using a Well Known Text string following\n\ncoords=POLYGON((x y,x1 y1,x2 y2,...,xn yn x y)).")]
    [OpenApiParameter("next", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Next page")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> GetOverlaysInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new OverlayFeaturesAreaRequest
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"]
        };
        var response =
            await waterResourceManager.Search<OverlayFeaturesSearchRequestBase, OverlayFeaturesSearchResponse>(request);

        return await CreateOkResponse(req, response);
    }
    
    [Function(nameof(GetOverlay))]
    [OpenApiOperation(operationId: "getOverlay", tags: [Tag], Summary = "Get a overlay feature",
        Description = "TODO: feature.",
        Visibility = OpenApiVisibilityType.Internal)]
    [OpenApiParameter("featureId", Type = typeof(string), In = ParameterLocation.Path,
        Required = true, Description = "The identifier of the feature.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> GetOverlay(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items/{featureId}")]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId,
        string featureId)
    {
        var request = new OverlayFeatureItemGetRequest
        {
            Id = featureId
        };
        var response = await waterResourceManager.Search<OverlayFeatureItemGetRequest, OverlayFeatureItemGetResponse>(request);
    
        return await CreateOkResponse(req, response.Feature);
    }
}