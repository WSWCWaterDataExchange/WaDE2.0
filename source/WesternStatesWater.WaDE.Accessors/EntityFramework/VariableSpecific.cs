using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class VariableSpecific : ControlledVocabularyBase
    {
        public VariableSpecific()
        {
            VariablesDim = new HashSet<VariablesDim>();
        }

        public virtual ICollection<VariablesDim> VariablesDim { get; set; }
    }
}