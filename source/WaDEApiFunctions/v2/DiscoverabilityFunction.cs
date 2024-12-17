using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WaDEApiFunctions.v2;

public class DiscoverabilityFunction(IWaterAllocationManager waterAllocationManager) : FunctionBase
{
    private const string PathBase = "v2/";

    [Function("LandingPage")]
    [OpenApiOperation(operationId: "getLandingPage", tags: ["Capabilities"], Summary = "Landing Page",
        Description = "The landing page provides links to the API definition.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Landing),
        Summary = "The response", Description = "The operation was executed successfully.")]
    public async Task<HttpResponseData> LandingPage(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "landingPage")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var mockLanding = new Landing
        {
            Title = "WaDE2.0",
            Description = "Water Data Exchange",
            Links =
            [
                new Link()
                {
                    Href = "https://wade-api.azurewebsites.net/ogcapi/",
                    Rel = "self",
                    Type = "application/json",
                    Title = "This document"
                },
                new Link()
                {
                    Href = "https://wade-api.azurewebsites.net/ogcapi/api",
                    Rel = "service-desc",
                    Type = "application/json",
                    Title = "The API definition"
                },
                new Link()
                {
                    Href = "https://wade-api.azurewebsites.net/ogcapi/conformance",
                    Rel = "conformance",
                    Type = "application/json",
                    Title = "OGC API conformance declaration"
                },
                new Link()
                {
                    Href = "https://wade-api.azurewebsites.net/ogcapi/collections",
                    Rel = "data",
                    Type = "application/json",
                    Title = "Resource collections"
                }
            ]
        };

        return await CreateOkResponse(req, mockLanding);
    }

    [Function("Conformance")]
    [OpenApiOperation(operationId: "getConformance", tags: ["Capabilities"], Summary = "Conformance",
        Description = "Conformance declaration for the OGC API.",
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
        bodyType: typeof(Conformance),
        Summary = "The URIs of all conformance classes supported by the server.",
        Description = "The operation was executed successfully.")]
    public async Task<HttpResponseData> Conformance(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = PathBase + "conformance")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        return await CreateOkResponse(req, new Conformance
        {
            ConformsTo =
            [
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core",
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30",
                "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/geojson"
            ]
        });
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
        var response = await waterAllocationManager.Collections();
        return await CreateOkResponse(req, response);
    }
}