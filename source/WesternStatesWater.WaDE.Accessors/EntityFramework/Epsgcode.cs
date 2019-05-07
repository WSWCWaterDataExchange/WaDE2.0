using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class Epsgcode
    {
        public Epsgcode()
        {
            ReportingUnitsDim = new HashSet<ReportingUnitsDim>();
            SitesDim = new HashSet<SitesDim>();
        }

        public string Name { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string State { get; set; }
        public string SourceVocabularyUri { get; set; }

        public virtual ICollection<ReportingUnitsDim> ReportingUnitsDim { get; set; }
        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}
