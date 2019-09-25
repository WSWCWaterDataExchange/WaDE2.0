using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IRegulatoryOverlayAccessor
    {
        Task<IEnumerable<RegulatoryReportingUnitsOrganization>> GetRegulatoryReportingUnitsAsync(RegulatoryOverlayFilters filters);
    }

}
