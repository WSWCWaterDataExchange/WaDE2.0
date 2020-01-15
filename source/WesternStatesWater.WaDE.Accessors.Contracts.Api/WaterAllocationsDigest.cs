using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class WaterAllocationsDigest
    {
        public DateTime AllocationPriorityDate { get; set; }
        public double? AllocationAmount { get; set; }
        public double? AllocationMaximum { get; set; }
        public IEnumerable<SiteDigest> Sites { get; set; }
    }
}