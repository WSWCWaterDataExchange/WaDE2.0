using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class DiscoverabilityFunction : FunctionBase
{
    private const string PathBase = "v2/";
    
    private readonly IMetadataManager _metadataManager;
    
    public DiscoverabilityFunction(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    [Function("LandingPage")]
    public async Task<HttpResponseData> LandingPage(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new DiscoveryMetadataGetRequest();
        var response = await _metadataManager.Load<DiscoveryMetadataGetRequest, DiscoveryMetadataGetResponse>(request);

        return await CreateResponse(req, response);
    }

    [Function("Conformance")]
    public async Task<HttpResponseData> Conformance(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "conformance")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new ConformanceMetadataGetRequest();
        var response = await _metadataManager.Load<ConformanceMetadataGetRequest, ConformanceMetadataGetResponse>(request);
        return await CreateResponse(req, response);
    }

    [Function("Collections")]
    public async Task<HttpResponseData> Collections(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "collections")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new CollectionsMetadataGetRequest();
        
        var response = await _metadataManager
            .Load<CollectionsMetadataGetRequest, CollectionsMetadataGetResponse>(request);
        
        return await CreateResponse(req, response);
    }
}