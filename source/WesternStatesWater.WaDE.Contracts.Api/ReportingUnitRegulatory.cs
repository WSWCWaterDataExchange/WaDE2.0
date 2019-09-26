using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class ReportingUnitRegulatory
    {
        public string ReportingUnitUUID { get; set; }
        public string ReportingUnitNativeID { get; set; }
        public string ReportingUnitName { get; set; }
        public string ReportingUnitTypeCV { get; set; }
        public string ReportingUnitUpdateDate { get; set; }
        public string ReportingUnitProductVersion { get; set; }
        public string StateCV { get; set; }
        public string EPSGCodeCV { get; set; }
        public string Geometry { get; set; }
    }
}
