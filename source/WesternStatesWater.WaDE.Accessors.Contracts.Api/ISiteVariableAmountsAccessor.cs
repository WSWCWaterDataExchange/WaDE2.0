using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface ISiteVariableAmountsAccessor
    {
        Task<IEnumerable<SiteVariableAmountsOrganization>> GetSiteVariableAmountsAsync(string variableCV, string variableSpecificCV, string beneficialUse, string siteUUID, string geometry, DateTime? startDate, DateTime? endDate);
    }
}
