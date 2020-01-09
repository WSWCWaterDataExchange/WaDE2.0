using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class WaterAllocationDigest
    {        
        public DateTime AllocationPriorityDate { get; set; }
        public double? AllocationAmount { get; set; }
        public double? AllocationMaximum { get; set; }
        public IEnumerable<SiteDigest> SitesLight { get; set; }
    }
}