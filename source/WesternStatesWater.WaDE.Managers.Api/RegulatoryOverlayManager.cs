using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public AccessorApi.IRegulatoryOverlayAccessor ApiRegulatoryOverlayAccessor { get; set; }

        async Task<IEnumerable<ManagerApi.RegulatoryReportingUnitsOrganization>> ManagerApi.IRegulatoryOverlayManager.GetRegulatoryReportingUnitsAsync(ManagerApi.RegulatoryOverlayFilters filters)
        {
            var results = await ApiRegulatoryOverlayAccessor.GetRegulatoryReportingUnitsAsync(filters.Map<AccessorApi.RegulatoryOverlayFilters>());
            return results.Select(a => a.Map<ManagerApi.RegulatoryReportingUnitsOrganization>());
        }
    }
}
