using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class RegulatoryOverlayFilters
    {
        public string ReportingUnitUUID { get; set; }
        public string RegulatoryOverlayUUID { get; set; }
        public string OrganizationUUID { get; set; }
        public DateTime? StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }
        public string RegulatoryStatusCV { get; set; }
        public string Geometry { get; set; }
    }
}
