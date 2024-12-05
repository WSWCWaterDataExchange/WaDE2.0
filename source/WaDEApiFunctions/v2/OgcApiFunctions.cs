using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;

namespace WaDEApiFunctions.v2;

public class OgcApiFunctions : FunctionBase
{
    [OpenApiOperation(operationId: "landingPage", tags: ["Capabilities"], Summary = "Landing Page",
        Description = "The landing page provides links to the API definition.",
        Visibility = OpenApiVisibilityType.Important)]
    // [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Landing),
        Summary = "The response", Description = "This returns the response")]
    [Function("LandingPage")]
    public static async Task<HttpResponseData> LandingPage(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ogcapi/LandingPage")]
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
                    Type = "application/json",
                    Title = "Resource collections"
                }
            ]
        };

        return await CreateOkResponse(req, mockLanding);
    }

    [Function("Conformance")]
    public static async Task<HttpResponseData> Conformance(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ogcapi/Conformance")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        return await CreateOkResponse(req, "Conformance!");
    }

    [Function("FeatureCollections")]
    public static async Task<HttpResponseData> FeatureCollections(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ogcapi/collections")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        return await CreateOkResponse(req, "Feature Collections!");
    }

    [Function("FeatureCollection")]
    public static async Task<HttpResponseData> FeatureCollection(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ogcapi/collections/{id}")]
        HttpRequestData req,
        FunctionContext executionContext,
        string id)
    {
        return await CreateOkResponse(req, "Feature Collection!");
    }

    [Function("Features")]
    public static async Task<HttpResponseData> Features(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ogcapi/collections/{id}/items")]
        HttpRequestData req,
        FunctionContext executionContext,
        string id)
    {
        return await CreateOkResponse(req, "Features!");
    }

    [Function("Feature")]
    public static async Task<HttpResponseData> Feature(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ogcapi/collections/{id}/items/{fid}")]
        HttpRequestData req,
        FunctionContext executionContext,
        string id,
        string fid)
    {
        return await CreateOkResponse(req, "Features!");
    }
}