using System.Threading.Tasks;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Managers.Api.Mapping;
using WesternStatesWater.WaDE.Managers.Mapping;

namespace WesternStatesWater.WaDE.Managers.Api.Handlers.V1;

internal class AggregatedAmountsSearchRequestHandler
    : IRequestHandler<AggregatedAmountsSearchRequest, AggregatedAmountsSearchResponse>
{
    private readonly AccessorApi.IAggregatedAmountsAccessor _aggregratedAmountsAccessor;

    public AggregatedAmountsSearchRequestHandler(
        AccessorApi.IAggregatedAmountsAccessor aggregratedAmountsAccessor)
    {
        _aggregratedAmountsAccessor = aggregratedAmountsAccessor;
    }

    public async Task<AggregatedAmountsSearchResponse> Handle(AggregatedAmountsSearchRequest request)
    {
        var filters = request.Filters.Map<AccessorApi.AggregatedAmountsFilters>();

        var dtoAmounts = await _aggregratedAmountsAccessor.GetAggregatedAmountsAsync(
            filters, request.StartIndex, request.RecordCount
        );

        var apiAmounts = dtoAmounts.Map<AggregatedAmounts>(options =>
            options.Items.Add(ApiProfile.GeometryFormatKey, request.OutputGeometryFormat)
        );

        return new AggregatedAmountsSearchResponse { AggregatedAmounts = apiAmounts };
    }
}