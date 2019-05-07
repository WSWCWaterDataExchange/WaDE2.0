using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IAggregatedAmountsAccessor
    {
        Task<IEnumerable<AggregatedAmountsOrganization>> GetAggregatedAmountsAsync(string variableCV, string variableSpecificCV, string beneficialUse, string reportingUnitUUID, string geometry, DateTime? startDate, DateTime? endDate);
    }
}
