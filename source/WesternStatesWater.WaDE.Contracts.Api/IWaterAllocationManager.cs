using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IWaterAllocationManager
    {
        Task<WaterAllocations> GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters, int startIndex, int recordCount);
    }
}
