using System;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Import
{
    public interface IWaterAllocationManager
    {
        Task<bool> LoadOrganizations(string runId);
        Task<bool> LoadWaterAllocations(string runId);
    }

    public interface IExcelFileConversionManager
    {
        Task ConvertExcelFileToJsonFiles(string container, string folder, string fileName);
    }
}
