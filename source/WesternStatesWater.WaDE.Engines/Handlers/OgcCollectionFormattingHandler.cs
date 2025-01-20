using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using Collection = WesternStatesWater.WaDE.Engines.Contracts.Ogc.Collection;

namespace WesternStatesWater.WaDE.Engines.Handlers;

/// <summary>
/// Loads the metadata for a specific collection.
/// See: CollectionsMetadataLoadHandler.cs for available collections. 
/// </summary>
public class OgcCollectionFormattingHandler(
    IConfiguration configuration,
    IRegulatoryOverlayAccessor regulatoryOverlayAccessor,
    ISiteVariableAmountsAccessor siteVariableAmountsAccessor,
    IWaterAllocationAccessor allocationAccessor,
    ISiteAccessor siteAccessor
) : OgcFormattingHandlerBase(configuration), IRequestHandler<CollectionRequest, CollectionResponse>
{
    public async Task<CollectionResponse> Handle(CollectionRequest request)
    {
        Collection? collection;

        switch (GetCollectionId(request.RequestUri))
        {
            case Constants.SitesCollectionId:
                collection = CreateCollection(await siteAccessor.GetSiteMetadata());
                break;
            case Constants.TimeSeriesCollectionId:
                collection = CreateCollection(await siteVariableAmountsAccessor.GetSiteVariableAmountsMetadata());
                break;
            case Constants.RightsCollectionId:
                collection = CreateCollection(await allocationAccessor.GetAllocationMetadata());
                break;
            case Constants.OverlaysCollectionId:
                collection = CreateCollection(await regulatoryOverlayAccessor.GetOverlayMetadata());
                break;
            default:
                throw new NotSupportedException($"Collection {GetCollectionId(request.RequestUri)} not found.");
        }

        return new CollectionResponse
        {
            Collection = collection
        };
    }
}