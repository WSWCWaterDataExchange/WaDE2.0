using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IWaterAllocationManager
    {
        Task<WaterAllocations> GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat);
        Task<IEnumerable<WaterAllocationDigest>> GetSiteAllocationAmountsDigestAsync(SiteAllocationAmountsDigestFilters siteAllocationAmountsLightFilters, int startIndex, int recordCount);
        Task<OgcApi.CollectionsResponse> Collections();
    }
}
