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

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadOrganizations(string runId, int startIndex, int count)
        {
            var orgs = await ImportWaterAllocationFileAccessor.GetOrganizations(runId, startIndex, count);
            if (!orgs.Any())
            {
                return true;
            }
            return await ImportWaterAllocationAccessor.LoadOrganizations(runId, orgs);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetOrganizationsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetOrganizationsCount(runId);
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

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadAggregatedAmounts(string runId)
        {
            var aggregatedAmounts = await ImportWaterAllocationFileAccessor.GetAggregatedAmounts(runId);

            if (!aggregatedAmounts.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadAggregatedAmounts(runId, aggregatedAmounts);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadWaterSources(string runId)
        {
            var waterSources = await ImportWaterAllocationFileAccessor.GetWaterSources(runId);

            if (!waterSources.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadWaterSources(runId, waterSources);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadMethods(string runId)
        {
            var methods = await ImportWaterAllocationFileAccessor.GetMethods(runId);

            if (!methods.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadMethods(runId, methods);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadRegulatoryOverlays(string runId)
        {
            var regulatoryOverlays = await ImportWaterAllocationFileAccessor.GetRegulatoryOverlays(runId);

            if (!regulatoryOverlays.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadRegulatoryOverlays(runId, regulatoryOverlays);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadRegulatoryReportingUnits(string runId)
        {
            var regulatoryReportingUnits = await ImportWaterAllocationFileAccessor.GetRegulatoryReportingUnits(runId);

            if (!regulatoryReportingUnits.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadRegulatoryReportingUnits(runId, regulatoryReportingUnits);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadReportingUnits(string runId)
        {
            var reportingUnits = await ImportWaterAllocationFileAccessor.GetReportingUnits(runId);

            if (!reportingUnits.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadReportingUnits(runId, reportingUnits);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadSites(string runId)
        {
            var sites = await ImportWaterAllocationFileAccessor.GetSites(runId);

            if (!sites.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadSites(runId, sites);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadSiteSpecificAmounts(string runId)
        {
            var siteSpecificAmounts = await ImportWaterAllocationFileAccessor.GetSiteSpecificAmounts(runId);

            if (!siteSpecificAmounts.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadSiteSpecificAmounts(runId, siteSpecificAmounts);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadVariables(string runId)
        {
            var variables = await ImportWaterAllocationFileAccessor.GetVariables(runId);

            if (!variables.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadVariables(runId, variables);
        }
    }
}
