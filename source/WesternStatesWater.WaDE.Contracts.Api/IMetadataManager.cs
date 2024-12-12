using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Contracts.Api;

public interface IMetadataManager
{
    Task<SiteCollectionSearchResponse> Describe(SiteCollectionSearchRequest request);
}