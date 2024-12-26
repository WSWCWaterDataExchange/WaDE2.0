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

public class WaterSitesFunction(IMetadataManager metadataManager) : FunctionBase
{
    private const string PathBase = "v2/collections/sites/";

    private const string Tag = "Sites";

    [Function(nameof(GetSiteCollectionMetadata))]
    [OpenApiOperation(operationId: "getSiteCollection", tags: [Tag], Summary = "Site collection metadata",
        Description = "WaDE sites collection.",
        Visibility = OpenApiVisibilityType.Internal)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "Successful request", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> GetSiteCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest
        {
            CollectionId = Constants.SitesCollectionId
        };
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateOkResponse(req, response.Collection);
    }

    [Function(nameof(GetWaterSites))]
    [OpenApiOperation(operationId: "getWaterSites", tags: [Tag], Summary = "Get water site collection items",
        Description = "TODO: features of site.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter("limit", Type = typeof(int), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "The maximum number of items to return.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public static async Task<HttpResponseData> GetWaterSites(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items")]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        return await CreateOkResponse(req, "All water sites!");
    }

    [Function(nameof(GetWaterSite))]
    [OpenApiOperation(operationId: "getWaterSite", tags: [Tag], Summary = "Get a water site feature W",
        Description = "TODO: feature.",
        Visibility = OpenApiVisibilityType.Internal)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public static async Task<HttpResponseData> GetWaterSite(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items/{featureId}")]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId,
        string featureId)
    {
        return await CreateOkResponse(req, "A water site!");
    }
}