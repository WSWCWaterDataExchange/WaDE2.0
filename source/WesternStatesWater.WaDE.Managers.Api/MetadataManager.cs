using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Sites.Responses;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api;

internal partial class WaterResourceManager : ManagerBase, ManagerApi.IMetadataManager
{
    public async Task<SiteCollectionSearchResponse> Describe(SiteCollectionSearchRequest request)
    {
        return await ExecuteAsync<SiteCollectionSearchRequest, SiteCollectionSearchResponse>(request);
    }
}