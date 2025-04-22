using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V1;

public class OverlayResourceSearchRequestHandler
    : IRequestHandler<OverlayResourceSearchRequest, OverlayResourceSearchResponse>
{
    private readonly AccessorApi.IRegulatoryOverlayAccessor _regulatoryOverlayAccessor;

    public OverlayResourceSearchRequestHandler(
        AccessorApi.IRegulatoryOverlayAccessor regulatoryOverlayAccessor)
    {
        _regulatoryOverlayAccessor = regulatoryOverlayAccessor;
    }

    public async Task<OverlayResourceSearchResponse> Handle(OverlayResourceSearchRequest request)
    {
        var filters = request.Filters.Map<AccessorApi.OverlayFilters>();

        var dtoReportingUnits = await _regulatoryOverlayAccessor.GetRegulatoryReportingUnitsAsync(
            filters, request.StartIndex, request.RecordCount
        );

        var apiReportingUnits = dtoReportingUnits.Map<ManagerApi.RegulatoryReportingUnits>(a =>
            a.Items.Add(ApiProfile.GeometryFormatKey, request.OutputGeometryFormat)
        );

        return new OverlayResourceSearchResponse { ReportingUnits = apiReportingUnits };
    }
}