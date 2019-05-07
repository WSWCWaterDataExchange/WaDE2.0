using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IWaterAllocationAccessor
    {
        Task<IEnumerable<WaterAllocationOrganization>> GetSiteAllocationAmountsAsync(string siteUuid, string beneficialUse, string geometry, DateTime? startPriorityDate, DateTime? endPriorityDate);
    }
}
