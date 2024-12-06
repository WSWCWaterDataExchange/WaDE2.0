using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api
{
    internal partial class WaterResourceManager : IRegulatoryOverlayManager
    {
        async Task<RegulatoryReportingUnits> IRegulatoryOverlayManager.GetRegulatoryReportingUnitsAsync(RegulatoryOverlayFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            var results = await _regulatoryOverlayAccessor.GetRegulatoryReportingUnitsAsync(filters.Map<AccessorApi.RegulatoryOverlayFilters>(), startIndex, recordCount);
            return results.Map<RegulatoryReportingUnits>(a => a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }
    }
}
