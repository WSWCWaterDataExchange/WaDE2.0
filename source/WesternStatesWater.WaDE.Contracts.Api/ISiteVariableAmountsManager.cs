using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface ISiteVariableAmountsManager
    {
        Task<IEnumerable<SiteVariableAmountsOrganization>> GetSiteVariableAmountsAsync(SiteVariableAmountsFilters filters);
    }
}
