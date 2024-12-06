using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api
{
    internal partial class WaterResourceManager : IAggregatedAmountsManager
    {   
        async Task<AggregatedAmounts> IAggregatedAmountsManager.GetAggregatedAmountsAsync(AggregatedAmountsFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            var results = await _aggregratedAmountsAccessor.GetAggregatedAmountsAsync(filters.Map<AccessorApi.AggregatedAmountsFilters>(), startIndex, recordCount);
            return results.Map<AggregatedAmounts>(a => a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }
    }
}
