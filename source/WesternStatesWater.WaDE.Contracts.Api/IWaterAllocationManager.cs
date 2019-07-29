using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IWaterAllocationManager
    {
        Task<IEnumerable<WaterAllocationOrganization>> GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters);
    }
}
