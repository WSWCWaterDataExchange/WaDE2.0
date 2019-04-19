using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IWaterAllocationFileAccessor
    {
        Task<List<Organization>> GetOrganizations(string runId);
        Task<List<WaterAllocation>> GetWaterAllocations(string runId);
        Task<List<AggregatedAmount>> GetAggregations(string runId);
        Task<List<AggregatedAmount>> GetMethods(string runId);
        Task<List<AggregatedAmount>> GetRegulatoryOverlays(string runId);
        Task<List<AggregatedAmount>> GetReportingUnits(string runId);
        Task<List<AggregatedAmount>> GetSites(string runId);
        Task<List<AggregatedAmount>> GetSiteSpecificAmounts(string runId);
        Task<List<AggregatedAmount>> GetVariables(string runId);
        Task<List<AggregatedAmount>> GetWaterSources(string runId);
    }

    public interface IBlobFileAccessor
    {
        Task<Stream> GetBlobData(string containter, string path);

        Task SaveBlobData(string containter, string path, byte[] data);
    }
}
