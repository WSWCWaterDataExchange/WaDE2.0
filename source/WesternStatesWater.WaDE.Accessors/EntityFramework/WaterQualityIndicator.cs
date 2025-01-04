using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class WaterQualityIndicator : ControlledVocabularyBase
    {
        public WaterQualityIndicator()
        {
            WaterSourcesDim = new HashSet<WaterSourcesDim>();
        }

        public virtual ICollection<WaterSourcesDim> WaterSourcesDim { get; set; }
    }
}