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
        public long TimeframeStartId { get; set; }
        public long TimeframeEndId { get; set; }
        public string ReportYearTypeCv { get; set; }
        public string ReportYearStartMonth { get; set; }

        public virtual DateDim TimeframeEnd { get; set; }
        public virtual DateDim TimeframeStart { get; set; }
        public virtual ICollection<RegulatoryReportingUnitsFact> RegulatoryReportingUnitsFact { get; set; }
    }
}
