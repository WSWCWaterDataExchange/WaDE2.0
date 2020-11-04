using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class WaterAllocationDigest
    {        
        public DateTime AllocationPriorityDate { get; set; }
        public double? AllocationFlow_CFS { get; set; }
        public double? AllocationVolume_AF { get; set; }
        public IEnumerable<SiteDigest> Sites { get; set; }
    }
}