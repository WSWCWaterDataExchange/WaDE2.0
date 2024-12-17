using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public interface IRegulatoryOverlayAccessor
    {
        Task<RegulatoryReportingUnits> GetRegulatoryReportingUnitsAsync(RegulatoryOverlayFilters filters,
            int startIndex, int recordCount);

        Task<OverlayMetadata> GetOverlayMetadata();
    }
}