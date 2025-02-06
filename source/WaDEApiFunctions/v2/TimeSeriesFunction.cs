using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class TimeSeriesFunction(IMetadataManager metadataManager, IWaterResourceManager waterResourceManager)
    : FunctionBase
{
    private const string PathBase = "v2/collections/timeSeries/";

    [Function(nameof(GetTimeSeriesCollectionMetadata))]
    public async Task<HttpResponseData> GetTimeSeriesCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest();
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateResponse(req, response);
    }

    [Function(nameof(SearchTimeSeries))]
    public async Task<HttpResponseData> SearchTimeSeries(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        // Modifying any query string parameters, will require updating swagger.json.
        var request = new TimeSeriesFeaturesItemRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            SiteUuids = req.Query["siteUuids"],
            States = req.Query["states"],
            WaterSourceTypes = req.Query["waterSourceTypes"],
            VariableTypes = req.Query["variableTypes"],
            DateTime = req.Query["datetime"]
        };
        var response =
            await waterResourceManager.Search<TimeSeriesFeaturesSearchRequestBase, TimeSeriesFeaturesSearchResponse>(request);

        return await CreateResponse(req, response);
    }

    [Function(nameof(SearchTimeSeriesInArea))]
    public async Task<HttpResponseData> SearchTimeSeriesInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        // Modifying any query string parameters, will require updating swagger.json.
        var request = new TimeSeriesFeaturesAreaRequest()
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"]
        };
        var response =
            await waterResourceManager.Search<TimeSeriesFeaturesSearchRequestBase, TimeSeriesFeaturesSearchResponse>(request);

        return await CreateResponse(req, response);
    }
    
    [Function(nameof(GetTimeSeries))]
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

        return await CreateResponse(req, response);
    }
}