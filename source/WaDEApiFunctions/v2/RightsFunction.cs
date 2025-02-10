using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class RightsFunction(IMetadataManager metadataManager, IWaterResourceManager waterResourceManager) : FunctionBase
{
    private const string PathBase = "v2/collections/rights";

    [Function(nameof(GetRightsCollectionMetadata))]
    public async Task<HttpResponseData> GetRightsCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest();
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateResponse(req, response, r => r.Collection);
    }

    [Function(nameof(GetRights))]
    public async Task<HttpResponseData> GetRights(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "/items")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var unmatchedResponse = await CheckUnmatchedParameters<RightFeaturesItemRequest>(req);
        if (unmatchedResponse != null)
        {
            return unmatchedResponse;
        }
        
        // Modifying any query string parameters, will require updating swagger.json.
        var request = new RightFeaturesItemRequest
        {
            Bbox = req.Query["bbox"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
            DateTime = req.Query["datetime"],
            AllocationUuids = req.Query["allocationUuids"],
            SiteUuids = req.Query["siteUuids"],
            States = req.Query["states"],
            WaterSourceTypes = req.Query["waterSourceTypes"],
            BeneficialUses = req.Query["beneficialUses"]
        };
        var response = await waterResourceManager.Search<RightFeaturesSearchRequestBase, RightFeaturesSearchResponse>(request);
        
        return await CreateResponse(req, response);
    }
    
    [Function(nameof(GetRightsInArea))]
    public async Task<HttpResponseData> GetRightsInArea(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "/area")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var unmatchedResponse = await CheckUnmatchedParameters<RightFeaturesAreaRequest>(req);
        if (unmatchedResponse != null)
        {
            return unmatchedResponse;
        }
        
        // Modifying any query string parameters, will require updating swagger.json.
        var request = new RightFeaturesAreaRequest
        {
            Coords = req.Query["coords"],
            Limit = req.Query["limit"],
            Next = req.Query["next"],
        };
        var response = await waterResourceManager.Search<RightFeaturesSearchRequestBase, RightFeaturesSearchResponse>(request);
        
        return await CreateResponse(req, response);
    }
    
    [Function(nameof(GetWaterRight))]
    public async Task<HttpResponseData> GetWaterRight(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "/items/{featureId}")]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId,
        string featureId)
    {
        var request = new RightFeatureItemGetRequest
        {
            Id = featureId
        };
        var response = await waterResourceManager.Search<RightFeatureItemGetRequest, RightFeatureItemGetResponse>(request);
    
        return await CreateResponse(req, response);
    }
}