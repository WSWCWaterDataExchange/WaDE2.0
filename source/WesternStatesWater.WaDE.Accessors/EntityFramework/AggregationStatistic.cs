using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class AggregationStatistic : ControlledVocabularyBase
    {
        public AggregationStatistic()
        {
            VariablesDim = new HashSet<VariablesDim>();
        }

        public virtual ICollection<VariablesDim> VariablesDim { get; set; }
    }
}
