using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class RegulatoryOverlay
    {
        public long RegulatoryOverlayID { get; set; }
        public string RegulatoryStatusCV { get; set; }
        public string OversightAgency { get; set; }
        public string RegulatoryDescription { get; set; }
        public DateTime StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }
        public string RegulatoryStatuteLink { get; set; }
        public string RegulatoryOverlayUUID { get; set; }
    }
}
