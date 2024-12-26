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

public class RightsFunction(IMetadataManager metadataManager) : FunctionBase
{
    private const string PathBase = "v2/collections/rights";
    private const string Tag = "Rights";

    [Function(nameof(GetRightsCollectionMetadata))]
    [OpenApiOperation(operationId: "getRightsCollection", tags: [Tag], Summary = "Rights collection metadata",
        Description = "WaDE water rights collection",
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
    public async Task<HttpResponseData> GetRightsCollectionMetadata(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase)]
        HttpRequestData req,
        FunctionContext executionContext,
        string collectionId)
    {
        var request = new CollectionMetadataGetRequest
        {
            CollectionId = Constants.RightsCollectionId
        };
        var response = await metadataManager.Load<CollectionMetadataGetRequest, CollectionMetadataGetResponse>(request);
        return await CreateOkResponse(req, response.Collection);
    }
}