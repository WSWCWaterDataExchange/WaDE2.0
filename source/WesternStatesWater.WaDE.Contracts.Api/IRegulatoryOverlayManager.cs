using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public interface IRegulatoryOverlayManager
    {
        Task<RegulatoryReportingUnits> GetRegulatoryReportingUnitsAsync(RegulatoryOverlayFilters filters, int startIndex, int recordCount);
    }
}
