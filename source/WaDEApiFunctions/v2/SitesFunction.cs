using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class WaterSitesFunction(
    IMetadataManager metadataManager,
    IWaterResourceManager waterResourceManager) : FunctionBase
{
    private const string PathBase = "v2/collections/sites/";
    
    [Function(nameof(GetSiteCollectionMetadata))]
    public async Task<HttpResponseData> GetSiteCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest();
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateResponse(req, response, r => r.Collection);
    }

    [Function(nameof(GetWaterSites))]
    public async Task<HttpResponseData> GetWaterSites(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var unmatchedResponse = await CheckUnmatchedParameters<SiteFeaturesItemRequest>(req);
        if (unmatchedResponse != null)
        {
            return unmatchedResponse;
        }
        
        // Modifying any query string parameters, will require updating swagger.json.
        var request = new SiteFeaturesItemRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            SiteUuids = req.Query["siteUuids"],
            OverlayUuids = req.Query["overlayUuids"],
            AllocationUuids = req.Query["allocationUuids"],
            SiteTypes = req.Query["siteTypes"],
            States = req.Query["states"],
            WaterSourceTypes = req.Query["waterSourceTypes"]
        };
        var response =
            await waterResourceManager.Search<SiteFeaturesSearchRequestBase, SiteFeaturesSearchResponse>(request);

        return await CreateResponse(req, response);
    }

    [Function(nameof(GetWaterSitesInArea))]
    public async Task<HttpResponseData> GetWaterSitesInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var unmatchedResponse = await CheckUnmatchedParameters<SiteFeaturesAreaRequest>(req);
        if (unmatchedResponse != null)
        {
            return unmatchedResponse;
        }
        
        // Modifying any query string parameters, will require updating swagger.json.
        var request = new SiteFeaturesAreaRequest
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            SiteUuids = req.Query["siteUuids"],
            OverlayUuids = req.Query["overlayUuids"],
            AllocationUuids = req.Query["allocationUuids"],
            SiteTypes = req.Query["siteTypes"],
            States = req.Query["states"],
            WaterSourceTypes = req.Query["waterSourceTypes"]
        };
        var response =
            await waterResourceManager.Search<SiteFeaturesSearchRequestBase, SiteFeaturesSearchResponse>(request);

        return await CreateResponse(req, response);
    }

    [Function(nameof(GetWaterSite))]
    public async Task<HttpResponseData> GetWaterSite(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "items/{featureId}")]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId,
        string featureId)
    {
        var request = new SiteFeatureItemGetRequest
        {
            Id = featureId
        };
        var response = await waterResourceManager.Search<SiteFeatureItemGetRequest, SiteFeatureItemGetResponse>(request);

        return await CreateResponse(req, response);
    }
}