using System;
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
        async Task<IEnumerable<Contracts.Api.SiteVariableAmountsOrganization>> ManagerApi.ISiteVariableAmountsManager.GetSiteVariableAmountsAsync(string variableCV, string variableSpecificCV, string beneficialUse, string siteUUID, string geometry, DateTime? startDate, DateTime? endDate)
        {
            var results = await ApiSiteVariableAmountsAccessor.GetSiteVariableAmountsAsync(variableCV, variableSpecificCV, beneficialUse, siteUUID, geometry, startDate, endDate);
            return results.Select(a => a.Map<Contracts.Api.SiteVariableAmountsOrganization>());
        }
    }
}
