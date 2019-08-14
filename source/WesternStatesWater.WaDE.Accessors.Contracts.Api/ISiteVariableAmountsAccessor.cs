using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface ISiteVariableAmountsAccessor
    {
        Task<IEnumerable<SiteVariableAmountsOrganization>> GetSiteVariableAmountsAsync(SiteVariableAmountsFilters filters);
    }
}
