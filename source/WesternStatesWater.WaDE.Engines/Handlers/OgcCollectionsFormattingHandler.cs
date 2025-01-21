using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Common;
using WesternStatesWater.WaDE.Common.Contexts;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Engines.Handlers;

public sealed class OgcCollectionsFormattingHandler(
    IConfiguration configuration,
    IRegulatoryOverlayAccessor regulatoryOverlayAccessor,
    ISiteVariableAmountsAccessor siteVariableAmountsAccessor,
    IWaterAllocationAccessor allocationAccessor,
    ISiteAccessor siteAccessor,
    IContextUtility contextUtility
) : OgcFormattingHandlerBase(configuration),
    IRequestHandler<CollectionsRequest, CollectionsResponse>
{
    public async Task<CollectionsResponse> Handle(CollectionsRequest request)
    {
        var requestUri = contextUtility.GetRequiredContext<ApiContext>().RequestUri;
        if (requestUri is null)
        {
            throw new WaDEException($"{nameof(ApiContext.RequestUri)} cannot be null.");
        }
        
        var overlayMetadata = await regulatoryOverlayAccessor.GetOverlayMetadata();
        var allocationMetadata = await allocationAccessor.GetAllocationMetadata();
        var siteMetadata = await siteAccessor.GetSiteMetadata();
        var timeSeriesMetadata =
            await siteVariableAmountsAccessor.GetSiteVariableAmountsMetadata();

        return new CollectionsResponse
        {
            Collections =
            [
                CreateCollection(siteMetadata, "sites"),
                CreateCollection(allocationMetadata, "rights"),
                CreateCollection(overlayMetadata, "overlays"),
                CreateCollection(timeSeriesMetadata, "timeseries")
            ],
            Links =
            [
                new Link { Href = requestUri.AbsoluteUri, Rel = "self", Type = "application/json", Title = "This document as JSON" },
                new Link { Href = $"{OgcHost}", Rel = "root", Type = "application/json", Title = "The API landing page" }
            ]
        };
    }
}