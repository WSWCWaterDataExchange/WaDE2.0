using System.Collections.Generic;
using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V1;

public class SiteAllocationAmountsDigestSearchRequestHandler
    : IRequestHandler<SiteAllocationAmountsDigestSearchRequest, SiteAllocationAmountsDigestSearchResponse>
{
    private readonly AccessorApi.IWaterAllocationAccessor _waterAllocationAccessor;

    public SiteAllocationAmountsDigestSearchRequestHandler(AccessorApi.IWaterAllocationAccessor waterAllocationAccessor)
    {
        _waterAllocationAccessor = waterAllocationAccessor;
    }

    public async Task<SiteAllocationAmountsDigestSearchResponse> Handle(
        SiteAllocationAmountsDigestSearchRequest request
    )
    {
        var filters = request.Filters.Map<AccessorApi.SiteAllocationAmountsDigestFilters>();

        var dtoAllocationDigests = await _waterAllocationAccessor
            .GetSiteAllocationAmountsDigestAsync(filters, request.StartIndex, request.RecordCount);

        var apiAllocationDigests = dtoAllocationDigests.Map<IEnumerable<ManagerApi.WaterAllocationDigest>>();

        return new SiteAllocationAmountsDigestSearchResponse { WaterAllocationDigests = apiAllocationDigests };
    }
}