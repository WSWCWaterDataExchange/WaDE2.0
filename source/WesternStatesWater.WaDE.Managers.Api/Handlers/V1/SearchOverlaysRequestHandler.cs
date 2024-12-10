using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V1;

internal class SearchOverlayRequestHandler : IRequestHandler<SearchOverlaysRequest, SearchOverlaysResponse>
{
    public Task<SearchOverlaysResponse> Handle(SearchOverlaysRequest request)
    {
        return Task.FromResult(new SearchOverlaysResponse());
    }
}