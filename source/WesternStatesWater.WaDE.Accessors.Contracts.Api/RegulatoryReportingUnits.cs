using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class RegulatoryReportingUnits
    {
        public int TotalRegulatoryReportingUnitsCount { get; set; }
        public IEnumerable<RegulatoryReportingUnitsOrganization> Organizations { get; set; }
    }
}