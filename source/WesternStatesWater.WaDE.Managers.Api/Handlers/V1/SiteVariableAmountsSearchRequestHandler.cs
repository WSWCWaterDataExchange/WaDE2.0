using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V1;

public class SiteVariableAmountsSearchRequestHandler
    : IRequestHandler<SiteVariableAmountsSearchRequest, SiteVariableAmountsSearchResponse>
{
    private readonly AccessorApi.ISiteVariableAmountsAccessor _siteVariableAmountsAccessor;

    public SiteVariableAmountsSearchRequestHandler(AccessorApi.ISiteVariableAmountsAccessor siteVariableAmountsAccessor)
    {
        _siteVariableAmountsAccessor = siteVariableAmountsAccessor;
    }

    public async Task<SiteVariableAmountsSearchResponse> Handle(SiteVariableAmountsSearchRequest request)
    {
        var filters = request.Filters.Map<AccessorApi.SiteVariableAmountsFilters>();

        var dtoSiteVariableAmounts = await _siteVariableAmountsAccessor.GetSiteVariableAmountsAsync(
            filters, request.StartIndex, request.RecordCount
        );

        var apiSiteVariableAmounts = dtoSiteVariableAmounts.Map<ManagerApi.SiteVariableAmounts>(a =>
            a.Items.Add(ApiProfile.GeometryFormatKey, request.OutputGeometryFormat)
        );

        return new SiteVariableAmountsSearchResponse { SiteVariableAmounts = apiSiteVariableAmounts };
    }
}