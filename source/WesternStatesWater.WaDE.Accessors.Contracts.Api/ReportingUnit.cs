using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class ReportingUnit
    {
        public long ReportingUnitId { get; set; }
        public string ReportingUnitNativeID { get; set; }
        public string ReportingUnitUUID { get; set; }
        public string ReportingUnitName { get; set; }
        public string ReportingUnitTypeCV { get; set; }
        public DateTime? ReportingUnitUpdateDate { get; set; }
        public string ReportingUnitProductVersion { get; set; }
        public Geometry ReportingUnitGeometry { get; set; }
        public List<string> RegulatoryOverlayUUIDs { get; set; }
    }
}
