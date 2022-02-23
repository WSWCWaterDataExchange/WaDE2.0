using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Api
{
    public class AggregratedAmountsManager : ManagerApi.IAggregatedAmountsManager
    {
        public AggregratedAmountsManager(AccessorApi.IAggregatedAmountsAccessor apiAggregratedAmountsAccessor)
        {
            ApiAggregratedAmountsAccessor = apiAggregratedAmountsAccessor;
        }

        public AccessorApi.IAggregatedAmountsAccessor ApiAggregratedAmountsAccessor { get; set; }
        async Task<ManagerApi.AggregatedAmounts> ManagerApi.IAggregatedAmountsManager.GetAggregatedAmountsAsync(ManagerApi.AggregatedAmountsFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            var results = await ApiAggregratedAmountsAccessor.GetAggregatedAmountsAsync(filters.Map<AccessorApi.AggregatedAmountsFilters>(), startIndex, recordCount);
            return results.Map<ManagerApi.AggregatedAmounts>(a => a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }
    }
}
