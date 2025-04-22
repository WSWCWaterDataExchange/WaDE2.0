using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IRegulatoryOverlayAccessor
    {
        Task<OverlayReportingUnits> GetRegulatoryReportingUnitsAsync(OverlayFilters filters,
            int startIndex, int recordCount);

        Task<OverlayMetadata> GetOverlayMetadata();
        
        public Task<TResponse> Search<TRequest, TResponse>(TRequest request)
            where TRequest : SearchRequestBase
            where TResponse : SearchResponseBase;
    }
}