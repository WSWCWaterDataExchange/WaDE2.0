using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;

namespace WesternStatesWater.WaDE.Managers.Import
{
    public class WaterAllocationManager : ManagerImport.IWaterAllocationManager
    {
        public WaterAllocationManager(AccessorImport.IWaterAllocationAccessor importWaterAllocationAccessor, AccessorImport.IWaterAllocationFileAccessor importWaterAllocationFileAccessor)
        {
            ImportWaterAllocationAccessor = importWaterAllocationAccessor;
            ImportWaterAllocationFileAccessor = importWaterAllocationFileAccessor;
        }

        public AccessorImport.IWaterAllocationAccessor ImportWaterAllocationAccessor { get; set; }
        public AccessorImport.IWaterAllocationFileAccessor ImportWaterAllocationFileAccessor { get; set; }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadOrganizations(string runId)
        {
            var orgs = await ImportWaterAllocationFileAccessor.GetOrganizations(runId);
            if (!orgs.Any())
            {
                return true;
            }
            return await ImportWaterAllocationAccessor.LoadOrganizations(runId, orgs);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadWaterAllocations(string runId)
        {
            var waterAllocations = await ImportWaterAllocationFileAccessor.GetWaterAllocations(runId);
            if (!waterAllocations.Any())
            {
                return true;
            }
            return await ImportWaterAllocationAccessor.LoadWaterAllocation(runId, waterAllocations);
        }
    }
}
