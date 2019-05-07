using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class NhdnetworkStatus
    {
        public NhdnetworkStatus()
        {
            Nhdmetadata = new HashSet<Nhdmetadata>();
            SitesDim = new HashSet<SitesDim>();
        }

        public string Name { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string State { get; set; }
        public string SourceVocabularyUri { get; set; }

        public virtual ICollection<Nhdmetadata> Nhdmetadata { get; set; }
        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}
