using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Import
{
    public interface IWaterAllocationManager
    {
        Task<bool> LoadOrganizations(string runId, int startIndex, int count);
        Task<int> GetOrganizationsCount(string runId);

        Task<bool> LoadWaterAllocations(string runId, int startIndex, int count);
        Task<int> GetWaterAllocationsCount(string runId);

        Task<bool> LoadAggregatedAmounts(string runId, int startIndex, int count);
        Task<int> GetAggregatedAmountsCount(string runId);

        Task<bool> LoadMethods(string runId, int startIndex, int count);
        Task<int> GetMethodsCount(string runId);

        Task<bool> LoadRegulatoryOverlays(string runId, int startIndex, int count);
        Task<int> GetRegulatoryOverlaysCount(string runId);

        Task<bool> LoadReportingUnits(string runId, int startIndex, int count);
        Task<int> GetReportingUnitsCount(string runId);

        Task<bool> LoadSites(string runId, int startIndex, int count);
        Task<int> GetSitesCount(string runId);

        Task<bool> LoadSiteSpecificAmounts(string runId, int startIndex, int count);
        Task<int> GetSiteSpecificAmountsCount(string runId);

        Task<bool> LoadVariables(string runId, int startIndex, int count);
        Task<int> GetVariablesCount(string runId);

        Task<bool> LoadWaterSources(string runId, int startIndex, int count);
        Task<int> GetWaterSourcesCount(string runId);

        Task<bool> LoadRegulatoryReportingUnits(string runId, int startIndex, int count);
        Task<int> GetRegulatoryReportingUnitsCount(string runId);

        Task<bool> LoadPODSiteToPOUSiteRelationships(string runId, int startIndex, int count);
        Task<int> GetPODSiteToPOUSiteRelationshipsCount(string runId);

        
    }
}
