using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface ISiteVariableAmountsAccessor
    {
        Task<SiteVariableAmounts> GetSiteVariableAmountsAsync(SiteVariableAmountsFilters filters, int startIndex, int recordCount);
        Task<SiteVariableAmountsMetadata> GetSiteVariableAmountsMetadata();
    }
}
