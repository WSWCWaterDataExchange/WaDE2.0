using System.Collections.Generic;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Allocations.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Allocations.Responses;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IWaterAllocationAccessor
    {
        Task<WaterAllocations> GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters, int startIndex,
            int recordCount);

        Task<IEnumerable<WaterAllocationsDigest>> GetSiteAllocationAmountsDigestAsync(
            SiteAllocationAmountsDigestFilters siteAllocationAmountsLightFilters, int startIndex, int recordCount);

        Task<TResponse> Search<TRequest, TResponse>(TRequest request)
            where TRequest : AllocationSearchRequestBase
            where TResponse : AllocationSearchResponseBase;
    }
}