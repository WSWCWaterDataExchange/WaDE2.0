using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers
{
    public class WaterAllocationManager : ManagerApi.IWaterAllocationManager, ManagerImport.IWaterAllocationManager
    {
        public WaterAllocationManager()//AccessorApi.IWaterAllocationAccessor apiWaterAllocationAccessor, AccessorImport.IWaterAllocationAccessor importWaterAllocationAccessor, AccessorImport.IWaterAllocationFileAccessor importWaterAllocationFileAccessor)
        {
            //ApiWaterAllocationAccessor = apiWaterAllocationAccessor;
            //ImportWaterAllocationAccessor = importWaterAllocationAccessor;
            //ImportWaterAllocationFileAccessor = importWaterAllocationFileAccessor;
        }

        public AccessorApi.IWaterAllocationAccessor ApiWaterAllocationAccessor { get; set; }
        public AccessorImport.IWaterAllocationAccessor ImportWaterAllocationAccessor { get; set; }
        public AccessorImport.IWaterAllocationFileAccessor ImportWaterAllocationFileAccessor { get; set; }

        async Task<IEnumerable<Contracts.Api.AllocationAmounts>> ManagerApi.IWaterAllocationManager.GetSiteAllocationAmountsAsync(string variableSpecificCV, string siteUuid)
        {
            var results = await ApiWaterAllocationAccessor.GetSiteAllocationAmountsAsync(variableSpecificCV, siteUuid);
            return results.Select(a => a.Map<Contracts.Api.AllocationAmounts>());
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadOrganizations(string runId)
        {
            var orgs = await ImportWaterAllocationFileAccessor.GetOrganizations(runId);
            return await ImportWaterAllocationAccessor.LoadOrganizations(runId, orgs);
        }
    }
}
