using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IWaterAllocationAccessor
    {
        Task<bool> LoadOrganizations(string runId, IEnumerable<Organization> organizations);
        Task<bool> LoadWaterAllocation(string runId, IEnumerable<WaterAllocation> waterAllocations);
        Task<bool> LoadAggregatedAmounts(string runId, IEnumerable<AggregatedAmount> aggregatedAmounts);
        Task<bool> LoadMethods(string runId, IEnumerable<Method> methods);
        Task<bool> LoadRegulatoryOverlays(string runId, IEnumerable<RegulatoryOverlay> regulatoryOverlays);
        Task<bool> LoadReportingUnits(string runId, IEnumerable<ReportingUnit> reportingUnits);
        Task<bool> LoadSites(string runId, IEnumerable<Site> sites);
        Task<bool> LoadSiteSpecificAmounts(string runId, IEnumerable<SiteSpecificAmount> siteSpecificAmounts);
        Task<bool> LoadVariables(string runId, IEnumerable<Variable> variables);
        Task<bool> LoadLoadWaterSources(string runId, IEnumerable<WaterSource> waterSources);
    }
}
