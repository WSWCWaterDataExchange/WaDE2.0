using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class Variable : ControlledVocabularyBase
    {
        public Variable()
        {
            VariablesDim = new HashSet<VariablesDim>();
        }

        public virtual ICollection<VariablesDim> VariablesDim { get; set; }
    }
}