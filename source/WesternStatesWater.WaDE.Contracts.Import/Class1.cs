using System;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Import
{
    public interface IWaterAllocationManager
    {
        Task<bool> LoadOrganizations(string runId);
    }
}
