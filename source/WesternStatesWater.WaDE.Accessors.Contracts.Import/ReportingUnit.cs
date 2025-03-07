﻿using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class ReportingUnit
    {
        [NullValues("")]
        public string ReportingUnitUUID { get; set; }

        [NullValues("")]
        public string ReportingUnitNativeID { get; set; }

        [NullValues("")]
        public string ReportingUnitName { get; set; }

        [NullValues("")]
        public string ReportingUnitTypeCV { get; set; }

        [NullValues("")]
        public DateTime? ReportingUnitUpdateDate { get; set; }

        [NullValues("")]
        public string ReportingUnitProductVersion { get; set; }

        [NullValues("")]
        public string StateCV { get; set; }

        [NullValues("")]
        public string EPSGCodeCV { get; set; }

        [NullValues("")]
        public string Geometry { get; set; }
    }
}
