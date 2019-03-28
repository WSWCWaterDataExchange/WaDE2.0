using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IWaterAllocationFileAccessor
    {
        Task<List<Organization>> GetOrganizations(string runId);
        Task<List<WaterAllocation>> GetWaterAllocations(string runId);
    }

    public interface IBlobFileAccessor
    {
        Task<Stream> GetBlobData(string containter, string path);

        Task SaveBlobData(string containter, string path, byte[] data);
    }
}
