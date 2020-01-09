using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IWaterAllocationAccessor
    {
        Task<WaterAllocations> GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters, int startIndex, int recordCount);
        Task<IEnumerable<WaterAllocationsDigest>> GetSiteAllocationAmountsDigestAsync(SiteAllocationAmountsDigestFilters siteAllocationAmountsLightFilters, int startIndex, int recordCount);
    }
}
