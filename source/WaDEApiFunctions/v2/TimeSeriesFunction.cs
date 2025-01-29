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

public class TimeSeriesFunction(IMetadataManager metadataManager, IWaterResourceManager waterResourceManager)
    : FunctionBase
{
    private const string PathBase = "v2/collections/timeSeries/";
    private const string Tag = "Time Series";

    [Function(nameof(GetTimeSeriesCollectionMetadata))]
    [OpenApiOperation(operationId: "getTimeSeriesCollections", tags: [Tag], Summary = "Time Series collection metadata",
        Description = "WaDE time series collection.",
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
    public async Task<HttpResponseData> GetTimeSeriesCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest();
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateOkResponse(req, response.Collection);
    }

    [Function(nameof(SearchTimeSeries))]
    [OpenApiOperation(operationId: "searchTimeSeries", tags: [Tag], Summary = "Get time series collection items",
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
    [OpenApiParameter("siteUuids", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Comma separate list of Site UUIDs")]
    [OpenApiParameter("states", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "State abbreviations")]
    [OpenApiParameter("waterSourceTypes", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Water Source Types")]
    [OpenApiParameter("variableTypes", Type = typeof(string[]), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "Variable Types")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Collection),
        Summary = "TODO: summary of collection.", Description = "The operation was executed successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Bad request", Description = "The request was invalid.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json",
        bodyType: typeof(object),
        Summary = "Not found", Description = "The request was invalid.")]
    public async Task<HttpResponseData> SearchTimeSeries(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new TimeSeriesFeaturesItemRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            SiteUuids = req.Query["siteUuids"],
            States = req.Query["states"],
            WaterSourceTypes = req.Query["waterSourceTypes"],
            VariableTypes = req.Query["variableTypes"],
        };
        var response =
            await waterResourceManager.Search<TimeSeriesFeaturesSearchRequestBase, TimeSeriesFeaturesSearchResponse>(request);

        return await CreateOkResponse(req, response);
    }

    [Function(nameof(SearchTimeSeriesInArea))]
    [OpenApiOperation(operationId: "searchTimeSeriesInArea", tags: [Tag],
        Summary = "Get time series collection items in area.",
        Description = "TODO: features of site.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter("limit", Type = typeof(int), In = ParameterLocation.Query,
        Explode = false,
        Required = false, Description = "The maximum number of items to return.")]
    [OpenApiParameter("coords", Type = typeof(string), In = ParameterLocation.Query,
        Explode = false,
        Required = false,
        Description =
            "Only data that has a geometry that intersects the area defined by the polygon are selected.\n\nThe polygon is defined using a Well Known Text string following\n\ncoords=POLYGON((x y,x1 y1,x2 y2,...,xn yn x y)).")]
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
    public async Task<HttpResponseData> SearchTimeSeriesInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new TimeSeriesFeaturesAreaRequest()
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"]
        };
        var response =
            await waterResourceManager.Search<TimeSeriesFeaturesSearchRequestBase, TimeSeriesFeaturesSearchResponse>(request);

        return await CreateOkResponse(req, response);
    }
    
    [Function(nameof(GetTimeSeries))]
    [OpenApiOperation(operationId: "getTimeSeries", tags: [Tag], Summary = "Get a site time series",
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
    public async Task<HttpResponseData> GetTimeSeries(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items/{featureId}")]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId,
        string featureId)
    {
        var request = new TimeSeriesFeatureItemGetRequest
        {
            Id = featureId
        };
        var response = await waterResourceManager.Search<TimeSeriesFeatureItemGetRequest, TimeSeriesFeatureItemGetResponse>(request);

        return await CreateOkResponse(req, response.Feature);
    }
}