using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class CoordinateMethod : ControlledVocabularyBase
    {
        public CoordinateMethod()
        {
            SitesDim = new HashSet<SitesDim>();
        }

        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}