using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V2;

internal class SiteCollectionLoadHandler : IRequestHandler<SiteCollectionLoadRequest, SiteCollectionLoadResponse>
{
    public Task<SiteCollectionLoadResponse> Handle(SiteCollectionLoadRequest request)
    {
        throw new System.NotImplementedException();
    }
}