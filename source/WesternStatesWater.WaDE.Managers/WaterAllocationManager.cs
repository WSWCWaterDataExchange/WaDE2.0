using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers
{
    public class WaterAllocationManager : IWaterAllocationManager
    {
        public WaterAllocationManager(IWaterAllocationAccessor waterAllocationAccessor)
        {
            WaterAllocationAccessor = waterAllocationAccessor;
        }

        public IWaterAllocationAccessor WaterAllocationAccessor { get; set; }

        async Task<IEnumerable<Contracts.Api.AllocationAmounts>> IWaterAllocationManager.GetSiteAllocationAmountsAsync(string variableSpecificCV, string siteUuid)
        {
            var results = await WaterAllocationAccessor.GetSiteAllocationAmountsAsync(variableSpecificCV, siteUuid);
            return results.Select(a => a.Map<Contracts.Api.AllocationAmounts>());
        }
    }
}
