using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IAggregatedAmountsManager
    {
        Task<IEnumerable<AggregatedAmountsOrganization>> GetAggregatedAmountsAsync(AggregatedAmountsFilters filters);
    }
}
