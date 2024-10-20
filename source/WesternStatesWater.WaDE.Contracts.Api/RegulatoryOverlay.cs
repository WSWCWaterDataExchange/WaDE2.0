﻿using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class RegulatoryOverlay
    {
        public string RegulatoryOverlayID { get; set; }
        public string RegulatoryOverlayUUID { get; set; }
        public string RegulatoryStatusCV { get; set; }
        public string OversightAgency { get; set; }
        public string RegulatoryDescription { get; set; }
        public DateTime StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }
        public string RegulatoryStatuteLink { get; set; }
        public string RegulatoryOverlayTypeCV { get; set; }
        public string WaterSourceTypeCV { get; set; }
    }
}
