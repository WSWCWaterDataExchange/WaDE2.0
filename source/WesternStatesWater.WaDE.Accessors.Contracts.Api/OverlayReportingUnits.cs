using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class OverlayReportingUnits
    {
        public int TotalRegulatoryReportingUnitsCount { get; set; }
        public IEnumerable<OverlayReportingUnitsOrganization> Organizations { get; set; }
    }
}