using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class Nhdproduct : ControlledVocabularyBase
    {
        public Nhdproduct()
        {
            SitesDim = new HashSet<SitesDim>();
        }

        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}