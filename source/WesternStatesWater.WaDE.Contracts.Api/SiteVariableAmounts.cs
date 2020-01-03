using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class SiteVariableAmounts
    {
        public int TotalSiteVariableAmountsCount { get; set; }
        public IEnumerable<SiteVariableAmountsOrganization> Organizations { get; set; }
    }
}