using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class RegulatoryOverlayType : ControlledVocabularyBase
    {
        public RegulatoryOverlayType()
        {
            RegulatoryOverlayDim = new HashSet<RegulatoryOverlayDim>();
        }

        public virtual ICollection<RegulatoryOverlayDim> RegulatoryOverlayDim { get; set; }
    }
}