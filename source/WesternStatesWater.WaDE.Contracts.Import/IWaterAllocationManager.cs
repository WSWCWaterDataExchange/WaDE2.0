using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Import
{
    public interface IWaterAllocationManager
    {
        Task<bool> LoadOrganizations(string runId);
        Task<bool> LoadWaterAllocations(string runId);
        Task<bool> LoadAggregatedAmounts(string runId);
        Task<bool> LoadMethods(string runId);
        Task<bool> LoadRegulatoryOverlays(string runId);
        Task<bool> LoadReportingUnits(string runId);
        Task<bool> LoadSites(string runId);
        Task<bool> LoadSiteSpecificAmounts(string runId);
        Task<bool> LoadVariables(string runId);
        Task<bool> LoadWaterSources(string runId);
    }

    public interface IExcelFileConversionManager
    {
        Task ConvertExcelFileToJsonFiles(string container, string folder, string fileName);
    }
}
