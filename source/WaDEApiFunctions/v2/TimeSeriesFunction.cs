using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v2;

public class TimeSeriesFunction(IMetadataManager metadataManager) : FunctionBase
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
}