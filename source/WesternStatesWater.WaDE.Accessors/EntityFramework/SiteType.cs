using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class SiteType : ControlledVocabularyBase
    {
        public SiteType()
        {
            SitesDim = new HashSet<SitesDim>();
        }

        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}