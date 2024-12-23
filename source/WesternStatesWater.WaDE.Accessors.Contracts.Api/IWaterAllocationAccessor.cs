using System.Collections.Generic;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;

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