using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class RegulatoryReportingUnits
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string RegulatoryOverlayUUID { get; set; }

        [NullValues("")]
        public string ReportingUnitUUID { get; set; }

        [NullValues("")]
        public string DataPublicationDate { get; set; }
    }
}
