using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class SitesVariableAmountBridgeAllocationsFact
    {
        public long SitesVariableAmountAllocationsId { get; set; }
        public long? SiteVariableAmountId { get; set; }
        public long? AllocationId { get; set; }

        public virtual AllocationsDim Allocation { get; set; }
        public virtual SiteVariableAmountsFact SiteVariableAmount { get; set; }
    }
}
