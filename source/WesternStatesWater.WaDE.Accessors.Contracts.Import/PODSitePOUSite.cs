using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class PODSitePOUSite
    {
        [NullValues("")]
        public string PODSiteUUID { get; set; }

        [NullValues("")]
        public string POUSiteUUID { get; set; }

        [NullValues("")]
        public DateTime? StartDate { get; set; }

        [NullValues("")]
        public DateTime? EndDate { get; set; }
    }
}
