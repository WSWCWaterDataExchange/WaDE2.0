using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Api
{
    public class RegulatoryOverlayManager : ManagerApi.IRegulatoryOverlayManager
    {
        public RegulatoryOverlayManager(AccessorApi.IRegulatoryOverlayAccessor apiAggregratedAmountsAccessor)
        {
            ApiRegulatoryOverlayAccessor = apiAggregratedAmountsAccessor;
        }

        private AccessorApi.IRegulatoryOverlayAccessor ApiRegulatoryOverlayAccessor { get; set; }

        async Task<ManagerApi.RegulatoryReportingUnits> ManagerApi.IRegulatoryOverlayManager.GetRegulatoryReportingUnitsAsync(ManagerApi.RegulatoryOverlayFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            var results = await ApiRegulatoryOverlayAccessor.GetRegulatoryReportingUnitsAsync(filters.Map<AccessorApi.RegulatoryOverlayFilters>(), startIndex, recordCount);
            return results.Map<ManagerApi.RegulatoryReportingUnits>(a => a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }
    }
}
