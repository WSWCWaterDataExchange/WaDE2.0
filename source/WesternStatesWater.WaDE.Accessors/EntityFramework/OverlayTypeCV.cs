using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class OverlayTypeCV : ControlledVocabularyBase
    {
        public OverlayTypeCV()
        {
            OverlayDim = new HashSet<OverlayDim>();
        }

        public virtual ICollection<OverlayDim> OverlayDim { get; set; }
    }
}