using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.WaterRights.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.WaterRights.Responses;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Accessors.WaterRights.Handlers;

internal class WaterRightsExtentSearchHandler : IRequestHandler<WaterRightExtentSearchRequest, WaterRightExtentSearchResponse>
{
    public Task<WaterRightExtentSearchResponse> Handle(WaterRightExtentSearchRequest request)
    {
        throw new System.NotImplementedException();
    }
}