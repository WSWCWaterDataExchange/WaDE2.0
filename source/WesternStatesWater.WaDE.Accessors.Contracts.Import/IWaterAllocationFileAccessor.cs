using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IWaterAllocationFileAccessor
    {
        Task<List<Organization>> GetOrganizations(string runId);
        Task<List<WaterAllocation>> GetWaterAllocations(string runId);
        Task<List<AggregatedAmount>> GetAggregatedAmounts(string runId);
        Task<List<Method>> GetMethods(string runId);
        Task<List<RegulatoryOverlay>> GetRegulatoryOverlays(string runId);
        Task<List<ReportingUnit>> GetReportingUnits(string runId);
        Task<List<Site>> GetSites(string runId);
        Task<List<SiteSpecificAmount>> GetSiteSpecificAmounts(string runId);
        Task<List<Variable>> GetVariables(string runId);
        Task<List<WaterSource>> GetWaterSources(string runId);
    }
}
