using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api
{
    internal partial class WaterResourceManager : IWaterAllocationManager
    {
        async Task<Contracts.Api.OgcApi.CollectionsResponse> IWaterAllocationManager.Collections()
        {
            var request = new CollectionsRequest();
            var response = await _formattingEngine.Format<CollectionsRequest, CollectionsResponse>(request);
            return DtoMapper.Map<Contracts.Api.OgcApi.CollectionsResponse>(response);
        }
    }
}