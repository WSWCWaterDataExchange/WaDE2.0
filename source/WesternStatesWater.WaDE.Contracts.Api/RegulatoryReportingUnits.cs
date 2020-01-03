using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class RegulatoryReportingUnits
    {
        public int TotalRegulatoryReportingUnitsCount { get; set; }
        public IEnumerable<SiteVariableAmountsOrganization> Organizations { get; set; }
    }
}