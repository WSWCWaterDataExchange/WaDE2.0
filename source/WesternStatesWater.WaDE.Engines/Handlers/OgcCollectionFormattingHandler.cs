using Microsoft.Extensions.Configuration;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Common;
using WesternStatesWater.WaDE.Common.Contexts;
using WesternStatesWater.WaDE.Common.Ogc;
using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Utilities;

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
    ISiteAccessor siteAccessor,
    IContextUtility contextUtility
) : OgcFormattingHandlerBase(configuration), IRequestHandler<CollectionRequest, CollectionResponse>
{
    public async Task<CollectionResponse> Handle(CollectionRequest request)
    {
        var requestUri = contextUtility.GetRequiredContext<ApiContext>().RequestUri;
        if (requestUri is null)
        {
            throw new WaDEException($"{nameof(ApiContext.RequestUri)} cannot be null.");
        }
        
        Collection? collection;

        var collectionId = GetCollectionId(requestUri);
        switch (GetCollectionId(requestUri))
        {
            case Constants.SitesCollectionId:
                collection = CreateCollection(await siteAccessor.GetSiteMetadata(), collectionId);
                break;
            case Constants.TimeSeriesCollectionId:
                collection = CreateCollection(await siteVariableAmountsAccessor.GetSiteVariableAmountsMetadata(), collectionId);
                break;
            case Constants.RightsCollectionId:
                collection = CreateCollection(await allocationAccessor.GetAllocationMetadata(), collectionId);
                break;
            case Constants.OverlaysCollectionId:
                collection = CreateCollection(await regulatoryOverlayAccessor.GetOverlayMetadata(), collectionId);
                break;
            default:
                throw new NotSupportedException($"Collection {collectionId} not found.");
        }

        return new CollectionResponse
        {
            Collection = collection
        };
    }
}