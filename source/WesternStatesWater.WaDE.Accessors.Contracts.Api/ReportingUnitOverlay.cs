using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class ReportingUnitOverlay
    {
        public string ReportingUnitUUID { get; set; }
        public string ReportingUnitNativeID { get; set; }
        public string ReportingUnitName { get; set; }
        public string ReportingUnitTypeCV { get; set; }
        public string ReportingUnitUpdateDate { get; set; }
        public string ReportingUnitProductVersion { get; set; }
        public string StateCV { get; set; }
        public string EPSGCodeCV { get; set; }
        public Geometry Geometry { get; set; }
    }
}
