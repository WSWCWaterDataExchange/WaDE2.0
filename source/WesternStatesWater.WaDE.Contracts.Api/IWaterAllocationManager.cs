using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{ 
    public interface IWaterAllocationManager
    {
        Task<IEnumerable<WaterAllocationOrganization>> GetSiteAllocationAmountsAsync(string variableSpecificCV, string siteUuid, string beneficialUse, string geometry);
    }
}
