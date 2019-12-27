using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class WaterAllocations
    {
        public int TotalCount { get; set; }
        public IEnumerable<WaterAllocationOrganization> WaterAllocationOrganizations { get; set; }
    }
}