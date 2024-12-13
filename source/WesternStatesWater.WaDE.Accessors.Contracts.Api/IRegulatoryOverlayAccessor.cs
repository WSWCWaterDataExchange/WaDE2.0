using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Overlays.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.Overlays.Responses;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IRegulatoryOverlayAccessor
    {
        Task<RegulatoryReportingUnits> GetRegulatoryReportingUnitsAsync(RegulatoryOverlayFilters filters,
            int startIndex, int recordCount);

        Task<TResponse> Search<TRequest, TResponse>(TRequest request)
            where TRequest : OverlaySearchRequestBase
            where TResponse : OverlaySearchResponseBase;
    }
}