using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class OverlayDim
    {
        public OverlayDim()
        {
            OverlayReportingUnitsFact = new HashSet<RegulatoryReportingUnitsFact>();
        }

        public long OverlayId { get; set; }
        public string OverlayUuid { get; set; }
        public string OverlayNativeId { get; set; }
        public string OverlayName { get; set; }
        public string OverlayDescription { get; set; }
        public string RegulatoryStatusCv { get; set; }
        public string OversightAgency { get; set; }
        public string RegulatoryStatute { get; set; }
        public string RegulatoryStatuteLink { get; set; }
        public DateTime StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }
        public string OverlayTypeCV { get; set; }
        public string WaterSourceTypeCV { get; set; }

        public virtual ICollection<RegulatoryReportingUnitsFact> OverlayReportingUnitsFact { get; set; }

        public virtual WaterSourceType WaterSourceType { get; set; }

        public virtual OverlayTypeCV OverlayType { get; set; }
        public virtual ICollection<OverlayBridgeSitesFact> OverlayBridgeSitesFact { get; set; }
    }
}
