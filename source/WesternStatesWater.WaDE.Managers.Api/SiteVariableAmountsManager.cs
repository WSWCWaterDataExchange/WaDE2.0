using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Managers.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Api
{
    public class SiteVariableAmountsManager : ManagerApi.ISiteVariableAmountsManager
    {
        public SiteVariableAmountsManager(AccessorApi.ISiteVariableAmountsAccessor apiSiteVariableAmountsAccessor)
        {
            ApiSiteVariableAmountsAccessor = apiSiteVariableAmountsAccessor;
        }

        public AccessorApi.ISiteVariableAmountsAccessor ApiSiteVariableAmountsAccessor { get; set; }
        async Task<IEnumerable<ManagerApi.SiteVariableAmountsOrganization>> ManagerApi.ISiteVariableAmountsManager.GetSiteVariableAmountsAsync(ManagerApi.SiteVariableAmountsFilters filters)
        {
            var results = await ApiSiteVariableAmountsAccessor.GetSiteVariableAmountsAsync(filters.Map<AccessorApi.SiteVariableAmountsFilters>());
            return results.Select(a => a.Map<ManagerApi.SiteVariableAmountsOrganization>());
        }
    }
}
