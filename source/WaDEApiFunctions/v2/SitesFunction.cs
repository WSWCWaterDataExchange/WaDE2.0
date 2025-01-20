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

public class WaterSitesFunction(
    IMetadataManager metadataManager,
    IWaterResourceManager waterResourceManager) : FunctionBase
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
    [OpenApiParameter("bbox", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Bounding box to filter results.")]
    [OpenApiParameter("next", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Next page")]
    [OpenApiParameter("siteTypes", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Site Types")]
    [OpenApiParameter("states", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "State abbreviations")]
    [OpenApiParameter("waterSourceTypes", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Water Source Types")]
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
        var request = new SiteFeaturesItemRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            SiteTypes = req.Query["siteTypes"],
            States = req.Query["states"],
            WaterSourceTypes = req.Query["waterSourceTypes"]
        };
        var response = await waterResourceManager.Search<SiteFeaturesSearchRequestBase, SiteFeaturesSearchResponse>(request);

        return await CreateOkResponse(req, response);
    }
    
    [Function(nameof(GetWaterSitesInArea))]
    [OpenApiOperation(operationId: "getWaterSitesInArea", tags: [Tag], Summary = "Return the data values for the data area defined by the query parameters",
        Description = "TODO: features of site.",
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
    public async Task<HttpResponseData> GetWaterSitesInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new SiteFeaturesAreaRequest
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"]
        };
        var response = await waterResourceManager.Search<SiteFeaturesSearchRequestBase, SiteFeaturesSearchResponse>(request);

        return await CreateOkResponse(req, response);
    }
    
    [Function(nameof(GetWaterSite))]
    [OpenApiOperation(operationId: "getWaterSite", tags: [Tag], Summary = "Get a water site feature W",
        Description = "TODO: feature.",
        Visibility = OpenApiVisibilityType.Internal)]
    [OpenApiParameter("featureId", Type = typeof(string), In = ParameterLocation.Path,
        Required = true, Description = "The identifier of the feature.")]
    [OpenApiParameter("test", Type = typeof(string[]), In = ParameterLocation.Query,
        Required = false, Description = "Testing parameter")]
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
        // TODO: This is just a placeholder functions, will be replaced with actual implementation.
        req.Headers.TryGetValues("X-WaDE-OriginalUrl", out var originalUrl);
        req.Headers.TryGetValues("X-WaDE-Url", out var url);
        req.Headers.TryGetValues("X-WaDE-Request", out var wadRequest);
        
        return await CreateOkResponse(req, new
        {
            FnUrl = req.Url,
            Original = originalUrl ?? ["N/A"],
            Url = url ?? ["N/A"],
            wadRequest = wadRequest ?? ["N/A"]
        });
    }
}