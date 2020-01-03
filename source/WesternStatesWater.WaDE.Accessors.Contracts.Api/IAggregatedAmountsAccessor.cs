using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IAggregatedAmountsAccessor
    {
        Task<AggregatedAmounts> GetAggregatedAmountsAsync(AggregatedAmountsFilters filters, int startIndex, int recordCount);
    }
}
