using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class NhdnetworkStatus : ControlledVocabularyBase
    {
        public NhdnetworkStatus()
        {
            SitesDim = new HashSet<SitesDim>();
        }

        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}