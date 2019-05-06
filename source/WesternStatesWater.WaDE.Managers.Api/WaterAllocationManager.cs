using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api
{
    public class WaterAllocationManager : ManagerApi.IWaterAllocationManager
    {
        public WaterAllocationManager(AccessorApi.IWaterAllocationAccessor apiWaterAllocationAccessor)
        {
            ApiWaterAllocationAccessor = apiWaterAllocationAccessor;
        }

        public AccessorApi.IWaterAllocationAccessor ApiWaterAllocationAccessor { get; set; }
        async Task<IEnumerable<Contracts.Api.WaterAllocationOrganization>> ManagerApi.IWaterAllocationManager.GetSiteAllocationAmountsAsync(string variableSpecificCV, string siteUuid, string beneficialUse, string geometry)
        {
            var results = await ApiWaterAllocationAccessor.GetSiteAllocationAmountsAsync(variableSpecificCV, siteUuid, beneficialUse, geometry);
            return results.Select(a => a.Map<Contracts.Api.WaterAllocationOrganization>());
        }
    }
}
