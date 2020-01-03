using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class WaterAllocations
    {
        public int TotalWaterAllocationsCount { get; set; }
        public IEnumerable<WaterAllocationOrganization> Organizations { get; set; }
    }
}