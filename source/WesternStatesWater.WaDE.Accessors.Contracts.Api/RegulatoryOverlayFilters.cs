using NetTopologySuite.Geometries;
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
        public DateTime? StartDataPublicationDate { get; set; }
        public DateTime? EndDataPublicationDate { get; set; }
        public string RegulatoryStatusCV { get; set; }
        public Geometry Geometry { get; set; }
        public string State { get; set; }
    }
}
