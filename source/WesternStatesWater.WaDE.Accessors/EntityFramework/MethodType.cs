using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class MethodType : ControlledVocabularyBase
    {
        public MethodType()
        {
            MethodsDim = new HashSet<MethodsDim>();
        }

        public virtual ICollection<MethodsDim> MethodsDim { get; set; }
    }
}