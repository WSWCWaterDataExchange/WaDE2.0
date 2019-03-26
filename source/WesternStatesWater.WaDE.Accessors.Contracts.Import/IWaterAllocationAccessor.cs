using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IWaterAllocationAccessor
    {
        Task<bool> LoadOrganizations(string runId, IEnumerable<Organization> organizations);
    }
}
