using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class ReportingUnit
    {
        public string ReportingUnitUUID { get; set; }
        public string ReportingUnitNativeID { get; set; }
        public string ReportingUnitName { get; set; }
        public string ReportingUnitTypeCV { get; set; }
        public DateTime? ReportingUnitUpdateDate { get; set; }
        public string ReportingUnitProductVersion { get; set; }
        public string StateCV { get; set; }
        public string EPSGCodeCV { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
