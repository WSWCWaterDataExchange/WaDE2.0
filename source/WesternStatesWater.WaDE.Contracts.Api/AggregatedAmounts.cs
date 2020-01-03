using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class AggregatedAmounts
    {
        public int TotalAggregatedAmountsCount { get; set; }
        public IEnumerable<AggregatedAmountsOrganization> Organizations { get; set; }
    }
}