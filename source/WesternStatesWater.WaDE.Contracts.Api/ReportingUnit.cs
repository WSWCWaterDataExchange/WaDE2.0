using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class ReportingUnit
    {
        public string ReportingUnitNativeID { get; set; }
        public string ReportingUnitUUID { get; set; }
        public string ReportingUnitName { get; set; }
        public string ReportingUnitTypeCV { get; set; }
        public DateTime? ReportingUnitUpdateDate { get; set; }
        public string ReportingUnitProductVersion { get; set; }
        public string ReportingUnitGeometry { get; set; }
    }
}
