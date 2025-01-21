using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
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
    [OpenApiOperation(operationId: "getLandingPage", tags: ["Capabilities"], Summary = "Landing Page",
        Description = "The landing page provides links to the API definition.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DiscoveryMetadataGetResponse),
        Summary = "The response", Description = "The operation was executed successfully.")]
    public async Task<HttpResponseData> LandingPage(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new DiscoveryMetadataGetRequest();
        var response = await _metadataManager.Load<DiscoveryMetadataGetRequest, DiscoveryMetadataGetResponse>(request);

        return await CreateOkResponse(req, response);
    }

    [Function("Conformance")]
    [OpenApiOperation(operationId: "getConformance", tags: ["Capabilities"], Summary = "Conformance",
        Description = "Conformance declaration for the OGC API.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(ConformanceMetadataGetResponse),
        Summary = "The URIs of all conformance classes supported by the server.",
        Description = "The operation was executed successfully.")]
    public async Task<HttpResponseData> Conformance(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "conformance")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new ConformanceMetadataGetRequest();
        var response = await _metadataManager.Load<ConformanceMetadataGetRequest, ConformanceMetadataGetResponse>(request);
        return await CreateOkResponse(req, response);
    }

    [Function("Collections")]
    [OpenApiOperation(operationId: "getCollections", tags: ["Discovery"], Summary = "Discovery",
        Description = "Collections",
        Visibility = OpenApiVisibilityType.Advanced)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(CollectionsResponse),
        Summary = "WaDE Collections", Description = "The operation was executed successfully.")]
    public async Task<HttpResponseData> Collections(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "collections")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var request = new CollectionsMetadataGetRequest();
        
        var response = await _metadataManager
            .Load<CollectionsMetadataGetRequest, CollectionsMetadataGetResponse>(request);
        
        return await CreateOkResponse(req, response);
    }
}