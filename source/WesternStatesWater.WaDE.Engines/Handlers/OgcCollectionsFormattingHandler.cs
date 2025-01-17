using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Common.Context;
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
) : OgcFormattingHandlerBase(configuration, contextUtility),
    IRequestHandler<CollectionsRequest, CollectionsResponse>
{
    public async Task<CollectionsResponse> Handle(CollectionsRequest request)
    {
        var overlayMetadata = await regulatoryOverlayAccessor.GetOverlayMetadata();
        var allocationMetadata = await allocationAccessor.GetAllocationMetadata();
        var siteMetadata = await siteAccessor.GetSiteMetadata();
        var timeSeriesMetadata =
            await siteVariableAmountsAccessor.GetSiteVariableAmountsMetadata();

        return new CollectionsResponse
        {
            Collections =
            [
                CreateCollection(siteMetadata),
                CreateCollection(allocationMetadata),
                CreateCollection(overlayMetadata),
                CreateCollection(timeSeriesMetadata)
            ],
            Links = new LinkBuilder(contextUtility.GetRequiredContext<ApiContext>())
                .AddCollections()
                .Build()
        };
    }
}