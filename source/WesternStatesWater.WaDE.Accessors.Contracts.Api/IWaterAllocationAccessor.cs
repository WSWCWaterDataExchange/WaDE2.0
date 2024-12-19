using System.Collections.Generic;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Responses;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IWaterAllocationAccessor
    {
        Task<WaterAllocations> GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters, int startIndex,
            int recordCount);

        Task<IEnumerable<WaterAllocationsDigest>> GetSiteAllocationAmountsDigestAsync(
            SiteAllocationAmountsDigestFilters siteAllocationAmountsLightFilters, int startIndex, int recordCount);

        Task<AllocationMetadata> GetAllocationMetadata();

        Task<TResponse> Search<TRequest, TResponse>(TRequest request)
            where TRequest : SearchRequestBase
            where TResponse : SearchResponseBase;
    }
}