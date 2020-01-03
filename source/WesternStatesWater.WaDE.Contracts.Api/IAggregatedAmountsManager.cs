using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IAggregatedAmountsManager
    {
        Task<AggregatedAmounts> GetAggregatedAmountsAsync(AggregatedAmountsFilters filters, int startIndex, int recordCount);
    }
}
