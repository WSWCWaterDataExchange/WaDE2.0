using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class RegulatoryOverlayType : ControlledVocabularyBase
    {
        public RegulatoryOverlayType()
        {
            RegulatoryOverlayDim = new HashSet<OverlayDim>();
        }

        public virtual ICollection<OverlayDim> RegulatoryOverlayDim { get; set; }
    }
}