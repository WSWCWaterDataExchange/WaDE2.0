using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api
{
    internal partial class WaterResourceManager : ISiteVariableAmountsManager
    {   
        async Task<SiteVariableAmounts> ISiteVariableAmountsManager.GetSiteVariableAmountsAsync(SiteVariableAmountsFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat)
        {
            var results = await _siteVariableAmountsAccessor.GetSiteVariableAmountsAsync(filters.Map<AccessorApi.SiteVariableAmountsFilters>(), startIndex, recordCount);
            return results.Map<SiteVariableAmounts>(a => a.Items.Add(ApiProfile.GeometryFormatKey, outputGeometryFormat));
        }
    }
}
