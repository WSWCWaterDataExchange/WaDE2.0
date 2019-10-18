using System.Collections.Generic;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IRegulatoryOverlayManager
    {
        Task<IEnumerable<RegulatoryReportingUnitsOrganization>> GetRegulatoryReportingUnitsAsync(RegulatoryOverlayFilters filters);
    }
}
