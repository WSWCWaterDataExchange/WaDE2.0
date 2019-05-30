using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface ISiteVariableAmountsManager
    {
        Task<IEnumerable<SiteVariableAmountsOrganization>> GetSiteVariableAmountsAsync(string variableCV, string variableSpecificCV, string beneficialUse, string siteUUID, string geometry, DateTime? startDate, DateTime? endDate);
    }
}
