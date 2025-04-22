using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class OverlayFilters
    {
        public string ReportingUnitUUID { get; set; }
        public string OverlayUUID { get; set; }
        public string OrganizationUUID { get; set; }
        public DateTime? StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }
        public DateTime? StartDataPublicationDate { get; set; }
        public DateTime? EndDataPublicationDate { get; set; }
        public string RegulatoryStatusCV { get; set; }
        public string Geometry { get; set; }
        public string State { get; set; }
    }
}
