using System.Collections.Generic;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Api
{
    public class WaterAllocationManager : ManagerApi.IWaterAllocationManager
    {
        public WaterAllocationManager(AccessorApi.IWaterAllocationAccessor apiWaterAllocationAccessor)
        {
            ApiWaterAllocationAccessor = apiWaterAllocationAccessor;
        }

        public AccessorApi.IWaterAllocationAccessor ApiWaterAllocationAccessor { get; set; }
        async Task<ManagerApi.WaterAllocations> ManagerApi.IWaterAllocationManager.GetSiteAllocationAmountsAsync(ManagerApi.SiteAllocationAmountsFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            var results = await ApiWaterAllocationAccessor.GetSiteAllocationAmountsAsync(filters.Map<AccessorApi.SiteAllocationAmountsFilters>(), startIndex, recordCount);
            return results.Map<ManagerApi.WaterAllocations>(a => a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }

        async Task<IEnumerable<ManagerApi.WaterAllocationDigest>> ManagerApi.IWaterAllocationManager.GetSiteAllocationAmountsDigestAsync(ManagerApi.SiteAllocationAmountsDigestFilters filters, int startIndex, int recordCount)
        {
            var results = await ApiWaterAllocationAccessor.GetSiteAllocationAmountsDigestAsync(filters.Map<AccessorApi.SiteAllocationAmountsDigestFilters>(), startIndex, recordCount);
            return results.Map<IEnumerable<ManagerApi.WaterAllocationDigest>>();
        }
    }
}
