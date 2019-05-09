using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        async Task<IEnumerable<ManagerApi.WaterAllocationOrganization>> ManagerApi.IWaterAllocationManager.GetSiteAllocationAmountsAsync(string siteUuid, string beneficialUse, string geometry, DateTime? startPriorityDate, DateTime? endPriorityDate)
        {
            var results = await ApiWaterAllocationAccessor.GetSiteAllocationAmountsAsync(siteUuid, beneficialUse, geometry, startPriorityDate, endPriorityDate);
            return results.Select(a => a.Map<ManagerApi.WaterAllocationOrganization>());
        }
    }
}
