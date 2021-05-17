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

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadWaterAllocations(string runId, int startIndex, int count)
        {
            var waterAllocations = await ImportWaterAllocationFileAccessor.GetWaterAllocations(runId, startIndex, count);
            if (!waterAllocations.Any())
            {
                return true;
            }
            return await ImportWaterAllocationAccessor.LoadWaterAllocation(runId, waterAllocations);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetWaterAllocationsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetWaterAllocationsCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadAggregatedAmounts(string runId, int startIndex, int count)
        {
            var aggregatedAmounts = await ImportWaterAllocationFileAccessor.GetAggregatedAmounts(runId, startIndex, count);

            if (!aggregatedAmounts.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadAggregatedAmounts(runId, aggregatedAmounts);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetAggregatedAmountsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetAggregatedAmountsCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadWaterSources(string runId, int startIndex, int count)
        {
            var waterSources = await ImportWaterAllocationFileAccessor.GetWaterSources(runId, startIndex, count);

            if (!waterSources.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadWaterSources(runId, waterSources);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetWaterSourcesCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetWaterSourcesCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadPODSiteToPOUSiteRelationships(string runId, int startIndex, int count)
        {
            var PODSitePOUSiteFacts = await ImportWaterAllocationFileAccessor.GetPODSiteToPOUSiteRelationships(runId, startIndex, count);

            if (!PODSitePOUSiteFacts.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadPODSitePOUSiteFact(runId, PODSitePOUSiteFacts);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetPODSiteToPOUSiteRelationshipsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetPODSiteToPOUSiteRelationshipsCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadMethods(string runId, int startIndex, int count)
        {
            var methods = await ImportWaterAllocationFileAccessor.GetMethods(runId, startIndex, count);

            if (!methods.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadMethods(runId, methods);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetMethodsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetMethodsCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadRegulatoryOverlays(string runId, int startIndex, int count)
        {
            var regulatoryOverlays = await ImportWaterAllocationFileAccessor.GetRegulatoryOverlays(runId, startIndex, count);

            if (!regulatoryOverlays.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadRegulatoryOverlays(runId, regulatoryOverlays);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetRegulatoryOverlaysCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetRegulatoryOverlaysCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadRegulatoryReportingUnits(string runId, int startIndex, int count)
        {
            var regulatoryReportingUnits = await ImportWaterAllocationFileAccessor.GetRegulatoryReportingUnits(runId, startIndex, count);

            if (!regulatoryReportingUnits.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadRegulatoryReportingUnits(runId, regulatoryReportingUnits);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetRegulatoryReportingUnitsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetRegulatoryReportingUnitsCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadReportingUnits(string runId, int startIndex, int count)
        {
            var reportingUnits = await ImportWaterAllocationFileAccessor.GetReportingUnits(runId, startIndex, count);

            if (!reportingUnits.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadReportingUnits(runId, reportingUnits);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetReportingUnitsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetReportingUnitsCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadSites(string runId, int startIndex, int count)
        {
            var sites = await ImportWaterAllocationFileAccessor.GetSites(runId, startIndex, count);

            if (!sites.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadSites(runId, sites);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetSitesCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetSitesCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadSiteSpecificAmounts(string runId, int startIndex, int count)
        {
            var siteSpecificAmounts = await ImportWaterAllocationFileAccessor.GetSiteSpecificAmounts(runId, startIndex, count);

            if (!siteSpecificAmounts.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadSiteSpecificAmounts(runId, siteSpecificAmounts);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetSiteSpecificAmountsCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetSiteSpecificAmountsCount(runId);
        }

        async Task<bool> ManagerImport.IWaterAllocationManager.LoadVariables(string runId, int startIndex, int count)
        {
            var variables = await ImportWaterAllocationFileAccessor.GetVariables(runId, startIndex, count);

            if (!variables.Any())
            {
                return true;
            }

            return await ImportWaterAllocationAccessor.LoadVariables(runId, variables);
        }

        async Task<int> ManagerImport.IWaterAllocationManager.GetVariablesCount(string runId)
        {
            return await ImportWaterAllocationFileAccessor.GetVariablesCount(runId);
        }
    }
}
