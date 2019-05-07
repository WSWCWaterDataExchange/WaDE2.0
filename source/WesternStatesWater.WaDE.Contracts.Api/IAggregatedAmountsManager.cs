using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IAggregatedAmountsManager
    {
        Task<IEnumerable<AggregatedAmountsOrganization>> GetAggregatedAmountsAsync(string variableCV, string variableSpecificCV, string beneficialUse, string reportingUnitUUID, string geometry, DateTime? startDate, DateTime? endDate);
    }
}
