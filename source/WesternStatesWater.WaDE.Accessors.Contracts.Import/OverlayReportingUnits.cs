using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class OverlayReportingUnits
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string OverlayUUID { get; set; }

        [NullValues("")]
        public string ReportingUnitUUID { get; set; }

        [NullValues("")]
        public string DataPublicationDate { get; set; }
    }
}
