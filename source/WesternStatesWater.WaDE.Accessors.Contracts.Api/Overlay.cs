using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class Overlay
    {
        public long OverlayID { get; set; }
        public string RegulatoryStatusCV { get; set; }
        public string OversightAgency { get; set; }
        public string OverlayDescription { get; set; }
        public DateTime StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }
        public string StatuteLink { get; set; }
        public string OverlayTypeCV { get; set; }
        public string WaterSourceTypeCV { get; set; }

        public string OverlayUUID { get; set; }
    }
}
