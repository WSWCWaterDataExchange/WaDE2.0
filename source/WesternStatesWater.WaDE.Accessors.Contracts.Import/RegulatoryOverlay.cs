using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class RegulatoryOverlay
    {
        [NullValues("")]
        public string OverlayUUID { get; set; }

        [NullValues("")]
        public string OverlayNativeID { get; set; }

        [NullValues("")]
        public string OverlayName { get; set; }

        [NullValues("")]
        public string OverlayDescription { get; set; }

        [NullValues("")]
        public string RegulatoryStatusCV { get; set; }

        [NullValues("")]
        public string OversightAgency { get; set; }

        [NullValues("")]
        public string RegulatoryStatute { get; set; }

        [NullValues("")]
        public string RegulatoryStatuteLink { get; set; }

        [NullValues("")]
        public DateTime? StatutoryEffectiveDate { get; set; }

        [NullValues("")]
        public DateTime? StatutoryEndDate { get; set; }

        [NullValues("")]
        public string OverlayTypeCV { get; set; }

        [NullValues("")]
        public string WaterSourceTypeCV { get; set; }
    }
}
