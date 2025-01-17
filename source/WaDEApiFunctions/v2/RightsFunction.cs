using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class RightsFunction(IMetadataManager metadataManager, IWaterResourceManager waterResourceManager) : FunctionBase
{
    private const string PathBase = "collections/rights";
    private const string Tag = "Rights";

    [Function(nameof(GetRightsCollectionMetadata))]
    [OpenApiOperation(operationId: "getRightsCollection", tags: [Tag], Summary = "Rights collection metadata",
        Description = "WaDE water rights collection",
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
    public async Task<HttpResponseData> GetRightsCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest
        {
            CollectionId = Constants.RightsCollectionId
        };
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateOkResponse(req, response.Collection);
    }

    [Function(nameof(GetRights))]
    [OpenApiOperation(operationId: "getRights", tags: [Tag], Summary = "Get water right collection items",
        Description = "Allocations",
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
    [OpenApiParameter("allocationUuids", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Allocation UUIDs")]
    [OpenApiParameter("siteUuids", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Site UUIDs")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> GetRights(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "/items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new RightFeaturesItemRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            AllocationUuids = req.Query["allocationUuids"],
            SiteUuids = req.Query["siteUuids"]
        };
        var response = await waterResourceManager.Search<RightFeaturesSearchRequestBase, RightFeaturesSearchResponse>(request);
        
        return await CreateOkResponse(req, response);
    }
    
    [Function(nameof(GetRightsInArea))]
    [OpenApiOperation(operationId: "getRightsInArea", tags: [Tag], Summary = "Get water right for a given area",
        Description = "Allocations",
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
    public async Task<HttpResponseData> GetRightsInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "/area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new RightFeaturesAreaRequest
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
        };
        var response = await waterResourceManager.Search<RightFeaturesSearchRequestBase, RightFeaturesSearchResponse>(request);
        
        return await CreateOkResponse(req, response);
    }
}