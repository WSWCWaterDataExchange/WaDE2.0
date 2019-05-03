using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class RegulatoryOverlayDim
    {
        public RegulatoryOverlayDim()
        {
            RegulatoryReportingUnitsFact = new HashSet<RegulatoryReportingUnitsFact>();
        }

        public long RegulatoryOverlayId { get; set; }
        public string RegulatoryOverlayUuid { get; set; }
        public string RegulatoryOverlayNativeId { get; set; }
        public string RegulatoryName { get; set; }
        public string RegulatoryDescription { get; set; }
        public string RegulatoryStatusCv { get; set; }
        public string OversightAgency { get; set; }
        public string RegulatoryStatute { get; set; }
        public string RegulatoryStatuteLink { get; set; }
        public DateTime StatutoryEffectiveDate { get; set; }
        public DateTime? StatutoryEndDate { get; set; }

        public virtual ICollection<RegulatoryReportingUnitsFact> RegulatoryReportingUnitsFact { get; set; }
    }
}
