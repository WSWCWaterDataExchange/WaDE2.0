using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests;
using WesternStatesWater.WaDE.Contracts.Api.Responses;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api
{
    internal partial class WaterResourceManager : IRegulatoryOverlayManager
    {
        async Task<RegulatoryReportingUnits> IRegulatoryOverlayManager.GetRegulatoryReportingUnitsAsync(
            RegulatoryOverlayFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            // Move to handler.
            var results =
                await _regulatoryOverlayAccessor.GetRegulatoryReportingUnitsAsync(
                    filters.Map<AccessorApi.RegulatoryOverlayFilters>(), startIndex, recordCount);
            return results.Map<RegulatoryReportingUnits>(a =>
                a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }

        Task<TResponse> IRegulatoryOverlayManager.Search<TRequest, TResponse>(TRequest request)
        {
            return ExecuteAsync<TRequest, TResponse>(request);
        }
    }
}