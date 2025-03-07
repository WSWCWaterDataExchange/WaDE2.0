using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class WaterAllocationBasis : ControlledVocabularyBase
    {
        public WaterAllocationBasis()
        {
            AllocationAmountsFact = new HashSet<AllocationAmountsFact>();
        }

        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
    }
}