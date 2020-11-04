using CsvHelper.Configuration.Attributes;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class RegulatoryOverlay
    {
        [NullValues("")]
        public string RegulatoryOverlayUUID { get; set; }

        [NullValues("")]
        public string RegulatoryOverlayNativeID { get; set; }

        [NullValues("")]
        public string RegulatoryName { get; set; }

        [NullValues("")]
        public string RegulatoryDescription { get; set; }

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
        public string RegulatoryOverlayTypeCV { get; set; }

        [NullValues("")]
        public string WaterSourceTypeCV { get; set; }
    }
}
