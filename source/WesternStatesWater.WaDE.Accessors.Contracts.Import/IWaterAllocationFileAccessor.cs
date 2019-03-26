using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IWaterAllocationFileAccessor
    {
        Task<List<Organization>> GetOrganizations(string runId);
    }
}
