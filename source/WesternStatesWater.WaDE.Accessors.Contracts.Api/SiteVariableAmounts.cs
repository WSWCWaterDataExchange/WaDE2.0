using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class SiteVariableAmounts
    {
        public int TotalSiteVariableAmountsCount { get; set; }
        public IEnumerable<SiteVariableAmountsOrganization> Organizations { get; set; }
    }
}