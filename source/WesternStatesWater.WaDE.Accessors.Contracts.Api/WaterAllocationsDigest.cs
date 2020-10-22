using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class WaterAllocationsDigest
    {
        public long AllocationAmountId { get; set; }
        public DateTime AllocationPriorityDate { get; set; }
        public double? AllocationFlow_CFS { get; set; }
        public double? AllocationMaximum { get; set; }
        public IEnumerable<SiteDigest> Sites { get; set; }
    }
}