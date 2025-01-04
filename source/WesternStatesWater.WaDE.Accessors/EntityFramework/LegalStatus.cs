using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class LegalStatus : ControlledVocabularyBase
    {
        public LegalStatus()
        {
            AllocationAmountsFact = new HashSet<AllocationAmountsFact>();
        }

        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
    }
}