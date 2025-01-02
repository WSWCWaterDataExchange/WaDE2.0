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

public class WaterSitesFunction(
    IMetadataManager metadataManager,
    ISiteManager siteManager) : FunctionBase
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
        var request = new CollectionMetadataRequest
        {
            CollectionId = Constants.SitesCollectionId
        };
        var response = await metadataManager.Load<CollectionMetadataRequest, CollectionMetadataResponse>(request);
        return await CreateOkResponse(req, response.Collection);
    }

    [Function(nameof(GetWaterSites))]
    [OpenApiOperation(operationId: "getWaterSites", tags: [Tag], Summary = "Get water site collection items",
        Description = "TODO: features of site.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter("limit", Type = typeof(int), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "The maximum number of items to return.")]
    [OpenApiParameter("bbox", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Bounding box to filter results.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> GetWaterSites(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new SiteFeaturesSearchRequest
        {
            Limit = string.IsNullOrWhiteSpace(req.Query["limit"]) ? null : int.Parse(req.Query["limit"]),
            Bbox = string.IsNullOrWhiteSpace(req.Query["bbox"]) ? null : ConvertBbox(req.Query["bbox"])
        };
        var response = (SiteFeaturesSearchResponse) await siteManager.Search<FeaturesSearchRequestBase, FeaturesSearchResponseBase>(request);

        return await CreateOkResponse(req, response);
    }
    
    private double[][]? ConvertBbox(string bbox)
    {
        if (string.IsNullOrWhiteSpace(bbox))
        {
            return null;
        }

        var bboxParts = bbox.Split(',');
        if (bboxParts.Length != 4)
        {
            return null;
        }

        return new[]
        {
            new[] { double.Parse(bboxParts[0]), double.Parse(bboxParts[1]), double.Parse(bboxParts[2]), double.Parse(bboxParts[3]) },
        };
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