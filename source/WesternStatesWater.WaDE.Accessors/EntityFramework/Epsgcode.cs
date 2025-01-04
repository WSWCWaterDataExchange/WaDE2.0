using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class Epsgcode : ControlledVocabularyBase
    {
        public Epsgcode()
        {
            ReportingUnitsDim = new HashSet<ReportingUnitsDim>();
            SitesDim = new HashSet<SitesDim>();
        }

        public virtual ICollection<ReportingUnitsDim> ReportingUnitsDim { get; set; }
        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}