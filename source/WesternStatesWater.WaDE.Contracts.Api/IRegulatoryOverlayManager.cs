using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Responses;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IRegulatoryOverlayManager
    {
        Task<RegulatoryReportingUnits> GetRegulatoryReportingUnitsAsync(RegulatoryOverlayFilters filters,
            int startIndex, int recordCount, GeometryFormat outputGeometryFormat);

        Task<TResponse> Search<TRequest, TResponse>(TRequest request)
            where TRequest : OverlaySearchRequestBase
            where TResponse : OverlaySearchResponseBase;
    }
}