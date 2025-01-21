using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Common.Contexts;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public class OgcDiscoveryFormattingHandler(IConfiguration configuration, IContextUtility contextUtility)
    : OgcFormattingHandlerBase(configuration), IRequestHandler<DiscoveryRequest, DiscoveryResponse>
{
    public Task<DiscoveryResponse> Handle(DiscoveryRequest request)
    {
        var requestUri = contextUtility.GetRequiredContext<ApiContext>().RequestUri;

        return Task.FromResult<DiscoveryResponse>(new DiscoveryResponse
        {
            Title = Configuration["OgcApi:Title"] ?? "WaDE 2.0 API",
            Description = Configuration["OgcApi:Description"] ?? "Water Data Exchange",
            Links =
            [
                new Link()
                {
                    Href = requestUri.ToString(),
                    Rel = "self",
                    Type = "application/json",
                    Title = "This document"
                },
                ServiceDescriptionLink,
                ServiceDocumentLink,
                ConformanceLink,
                CollectionsLink
            ]
        });
    }
}