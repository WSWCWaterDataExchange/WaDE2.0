using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        async Task<IEnumerable<ManagerApi.AggregatedAmountsOrganization>> ManagerApi.IAggregatedAmountsManager.GetAggregatedAmountsAsync(ManagerApi.AggregatedAmountsFilters filters)
        {
            var results = await ApiAggregratedAmountsAccessor.GetAggregatedAmountsAsync(filters.Map<AccessorApi.AggregatedAmountsFilters>());
            return results.Select(a => a.Map<ManagerApi.AggregatedAmountsOrganization>());
        }
    }
}
