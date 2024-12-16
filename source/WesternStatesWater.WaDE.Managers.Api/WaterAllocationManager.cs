using System.Collections.Generic;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;
using WesternStatesWater.WaDE.Engines.Contracts.Ogc.Responses;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api
{
    internal partial class WaterResourceManager : IWaterAllocationManager
    {   
        async Task<WaterAllocations> IWaterAllocationManager.GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            var results = await _waterAllocationAccessor.GetSiteAllocationAmountsAsync(filters.Map<AccessorApi.SiteAllocationAmountsFilters>(), startIndex, recordCount);
            return results.Map<WaterAllocations>(a => a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }

        async Task<IEnumerable<WaterAllocationDigest>> IWaterAllocationManager.GetSiteAllocationAmountsDigestAsync(SiteAllocationAmountsDigestFilters filters, int startIndex, int recordCount)
        {
            var results = await _waterAllocationAccessor.GetSiteAllocationAmountsDigestAsync(filters.Map<AccessorApi.SiteAllocationAmountsDigestFilters>(), startIndex, recordCount);
            return results.Map<IEnumerable<WaterAllocationDigest>>();
        }

        async Task<Contracts.Api.OgcApi.CollectionsResponse> IWaterAllocationManager.Collections()
        {
            var request = new CollectionsRequest();
            var response = await _formattingEngine.Format<CollectionsRequest, CollectionsResponse>(request);
            return DtoMapper.Map<Contracts.Api.OgcApi.CollectionsResponse>(response);
        }
    }
}
