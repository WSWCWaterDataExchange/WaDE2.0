using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class Nhdmetadata
    {
        public Nhdmetadata()
        {
            SitesDim = new HashSet<SitesDim>();
        }

        public long NhdmetadataId { get; set; }
        public string NhdnetworkStatusCv { get; set; }
        public string NhdproductCv { get; set; }
        public DateTime? NhdupdateDate { get; set; }
        public string NhdreachCode { get; set; }
        public string NhdmeasureNumber { get; set; }

        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}
