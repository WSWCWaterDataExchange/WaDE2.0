using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface ISiteVariableAmountsManager
    {
        Task<SiteVariableAmounts> GetSiteVariableAmountsAsync(SiteVariableAmountsFilters filters, int startIndex, int recordCount, GeometryFormat outputGeometryFormat);
    }
}
