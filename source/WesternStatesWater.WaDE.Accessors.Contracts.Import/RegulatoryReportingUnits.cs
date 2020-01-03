using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class RegulatoryReportingUnits
    {
        [NullValues("")]
        public string OrganizationName { get; set; }

        [NullValues("")]
        public string RegulatoryOverlayName { get; set; }

        [NullValues("")]
        public string ReportingUnitName { get; set; }

        [NullValues("")]
        public string DataPublicationDateID { get; set; }
    }
}
