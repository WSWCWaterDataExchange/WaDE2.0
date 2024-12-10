using System;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V1;

internal class SearchOverlayRequestHandler : IRequestHandler<SearchOverlaysRequest>
{
    public Task<ResponseBase> Handle(SearchOverlaysRequest request)
    {
        throw new NotImplementedException("Test test test");
    }
}