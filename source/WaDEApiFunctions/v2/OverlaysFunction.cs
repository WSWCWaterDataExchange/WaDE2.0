using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class OverlaysFunction(IMetadataManager metadataManager, IWaterResourceManager waterResourceManager)
    : FunctionBase
{
    private const string PathBase = "v2/collections/overlays/";

    [Function(nameof(GetOverlaysCollectionMetadata))]
    public async Task<HttpResponseData> GetOverlaysCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest();
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateResponse(req, response, r => r.Collection);
    }

    [Function(nameof(GetOverlays))]
    public async Task<HttpResponseData> GetOverlays(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var unmatchedResponse = await CheckUnmatchedParameters<OverlayFeaturesItemRequest>(req);
        if (unmatchedResponse != null)
        {
            return unmatchedResponse;
        }

        // Modifying any query string parameters, will require updating swagger.json.
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

        return await CreateResponse(req, response);
    }

    [Function(nameof(GetOverlaysInArea))]
    public async Task<HttpResponseData> GetOverlaysInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var unmatchedResponse = await CheckUnmatchedParameters<OverlayFeaturesAreaRequest>(req);
        if (unmatchedResponse != null)
        {
            return unmatchedResponse;
        }
        
        // Modifying any query string parameters, will require updating swagger.json.
        var request = new OverlayFeaturesAreaRequest
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"]
        };
        var response =
            await waterResourceManager.Search<OverlayFeaturesSearchRequestBase, OverlayFeaturesSearchResponse>(request);

        return await CreateResponse(req, response);
    }

    [Function(nameof(GetOverlay))]
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
        var response =
            await waterResourceManager.Search<OverlayFeatureItemGetRequest, OverlayFeatureItemGetResponse>(request);

        return await CreateResponse(req, response);
    }
}