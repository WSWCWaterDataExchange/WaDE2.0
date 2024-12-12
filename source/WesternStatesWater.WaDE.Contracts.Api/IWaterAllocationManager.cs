using System.Collections.Generic;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Responses;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IWaterAllocationManager
    {
        Task<WaterAllocations> GetSiteAllocationAmountsAsync(SiteAllocationAmountsFilters filters, int startIndex,
            int recordCount, GeometryFormat outputGeometryFormat);

        Task<IEnumerable<WaterAllocationDigest>> GetSiteAllocationAmountsDigestAsync(
            SiteAllocationAmountsDigestFilters siteAllocationAmountsLightFilters, int startIndex, int recordCount);

        Task<TResponse> Search<TRequest, TResponse>(TRequest request)
            where TRequest : AllocationRequestBase
            where TResponse : AllocationResponseBase;
    }
}