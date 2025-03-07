using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V1;

public class SiteAllocationAmountsSearchRequestHandler
    : IRequestHandler<SiteAllocationAmountsSearchRequest, SiteAllocationAmountsSearchResponse>
{
    private readonly AccessorApi.IWaterAllocationAccessor _waterAllocationAccessor;

    public SiteAllocationAmountsSearchRequestHandler(AccessorApi.IWaterAllocationAccessor waterAllocationAccessor)
    {
        _waterAllocationAccessor = waterAllocationAccessor;
    }

    public async Task<SiteAllocationAmountsSearchResponse> Handle(SiteAllocationAmountsSearchRequest request)
    {
        var filters = request.Filters.Map<AccessorApi.SiteAllocationAmountsFilters>();

        var dtoAmounts = await _waterAllocationAccessor.GetSiteAllocationAmountsAsync(
            filters, request.StartIndex, request.RecordCount
        );

        var apiAmounts = dtoAmounts.Map<ManagerApi.WaterAllocations>(options =>
            options.Items.Add(ApiProfile.GeometryFormatKey, request.OutputGeometryFormat)
        );

        return new SiteAllocationAmountsSearchResponse { WaterAllocations = apiAmounts };
    }
}