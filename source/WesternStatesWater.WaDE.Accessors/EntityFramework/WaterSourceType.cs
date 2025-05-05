using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class WaterSourceType : ControlledVocabularyBase
    {
        public WaterSourceType()
        {
            WaterSourcesDim = new HashSet<WaterSourcesDim>();
        }

        public virtual ICollection<WaterSourcesDim> WaterSourcesDim { get; set; }

        public virtual ICollection<OverlayDim> RegulatoryOverlayDim { get; set; }
    }
}