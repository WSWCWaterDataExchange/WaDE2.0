using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IWaterAllocationFileAccessor
    {
        Task<List<Organization>> GetOrganizations(string runId, int startIndex, int count);
        Task<int> GetOrganizationsCount(string runId);

        Task<List<WaterAllocation>> GetWaterAllocations(string runId, int startIndex, int count);
        Task<int> GetWaterAllocationsCount(string runId);

        Task<List<AggregatedAmount>> GetAggregatedAmounts(string runId, int startIndex, int count);
        Task<int> GetAggregatedAmountsCount(string runId);

        Task<List<Method>> GetMethods(string runId, int startIndex, int count);
        Task<int> GetMethodsCount(string runId);

        Task<List<RegulatoryOverlay>> GetRegulatoryOverlays(string runId, int startIndex, int count);
        Task<int> GetRegulatoryOverlaysCount(string runId);

        Task<List<RegulatoryReportingUnits>> GetRegulatoryReportingUnits(string runId, int startIndex, int count);
        Task<int> GetRegulatoryReportingUnitsCount(string runId);

        Task<List<ReportingUnit>> GetReportingUnits(string runId, int startIndex, int count);
        Task<int> GetReportingUnitsCount(string runId);

        Task<List<Site>> GetSites(string runId, int startIndex, int count);
        Task<int> GetSitesCount(string runId);

        Task<List<SiteSpecificAmount>> GetSiteSpecificAmounts(string runId, int startIndex, int count);
        Task<int> GetSiteSpecificAmountsCount(string runId);

        Task<List<Variable>> GetVariables(string runId, int startIndex, int count);
        Task<int> GetVariablesCount(string runId);

        Task<List<WaterSource>> GetWaterSources(string runId, int startIndex, int count);
        Task<int> GetWaterSourcesCount(string runId);

        Task<List<PODSitePOUSite>> GetPODSiteToPOUSiteRelationships(string runId, int startIndex, int count);
        Task<int> GetPODSiteToPOUSiteRelationshipsCount(string runId);
    }
}
