using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class WaterSourcesDim
    {
        public WaterSourcesDim()
        {
            AggregatedAmountsFact = new HashSet<AggregatedAmountsFact>();
            SitesDim = new HashSet<SitesDim>();
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
        }

        public long WaterSourceId { get; set; }
        public string WaterSourceUuid { get; set; }
        public string WaterSourceNativeId { get; set; }
        public string WaterSourceName { get; set; }
        public string WaterSourceTypeCv { get; set; }
        public string WaterQualityIndicatorCv { get; set; }
        public string GnisfeatureNameCv { get; set; }
        public Geometry Geometry { get; set; }

        public virtual GnisfeatureName GnisfeatureNameCvNavigation { get; set; }
        public virtual WaterQualityIndicator WaterQualityIndicatorCvNavigation { get; set; }
        public virtual WaterSourceType WaterSourceTypeCvNavigation { get; set; }
        public virtual ICollection<AggregatedAmountsFact> AggregatedAmountsFact { get; set; }
        public virtual ICollection<SitesDim> SitesDim { get; set; }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
    }
}
