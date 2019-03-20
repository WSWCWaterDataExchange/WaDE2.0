using System;
using System.Collections.Generic;
using System.Text;
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

        List<Contracts.Api.AllocationAmounts> IWaterAllocationManager.GetSiteAllocationAmounts(string variableSpecificCV, string siteUuid)
        {
            return WaterAllocationAccessor.GetSiteAllocationAmounts(variableSpecificCV, siteUuid).Map<List<Contracts.Api.AllocationAmounts>>();
        }
    }
}
