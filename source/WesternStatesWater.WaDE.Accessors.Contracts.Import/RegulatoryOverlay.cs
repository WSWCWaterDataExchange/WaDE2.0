using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class RegulatoryOverlay
    {
        public string RegulatoryOverlayUUID { get; set; }
        public string RegulatoryOverlayNativeID { get; set; }
        public string RegulatoryName { get; set; }
        public string RegulatoryDescription { get; set; }
        public string RegulatoryStatusCV { get; set; }
        public string OversightAgency { get; set; }
        public string RegulatoryStatute { get; set; }
        public string RegulatoryStatuteLink { get; set; }
        public DateTime? StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }
    }
}
