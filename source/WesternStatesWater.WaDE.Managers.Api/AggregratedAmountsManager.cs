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
        async Task<IEnumerable<Contracts.Api.AggregatedAmountsOrganization>> ManagerApi.IAggregatedAmountsManager.GetAggregatedAmountsAsync(string variableCV, string variableSpecificCV, string beneficialUse, string reportingUnitUUID, string geometry, DateTime? startDate, DateTime? endDate)
        {
            var results = await ApiAggregratedAmountsAccessor.GetAggregatedAmountsAsync(variableCV, variableSpecificCV, beneficialUse, reportingUnitUUID, geometry, startDate, endDate);
            return results.Select(a => a.Map<Contracts.Api.AggregatedAmountsOrganization>());
        }
    }
}
