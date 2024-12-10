using System.Threading.Tasks;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Managers.Api;

internal partial class WaterResourceManager
{
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    private readonly AccessorApi.IAggregatedAmountsAccessor _aggregratedAmountsAccessor;

    private readonly AccessorApi.IRegulatoryOverlayAccessor _regulatoryOverlayAccessor;

    private readonly AccessorApi.ISiteVariableAmountsAccessor _siteVariableAmountsAccessor;

    private readonly AccessorApi.IWaterAllocationAccessor _waterAllocationAccessor;

    public WaterResourceManager(
        IRequestHandlerResolver requestHandlerResolver,
        AccessorApi.IAggregatedAmountsAccessor aggregratedAmountsAccessor,
        AccessorApi.IRegulatoryOverlayAccessor regulatoryOverlayAccessor,
        AccessorApi.ISiteVariableAmountsAccessor siteVariableAmountsAccessor,
        AccessorApi.IWaterAllocationAccessor waterAllocationAccessor
    )
    {
        _requestHandlerResolver = requestHandlerResolver;
        _aggregratedAmountsAccessor = aggregratedAmountsAccessor;
        _regulatoryOverlayAccessor = regulatoryOverlayAccessor;
        _siteVariableAmountsAccessor = siteVariableAmountsAccessor;
        _waterAllocationAccessor = waterAllocationAccessor;
    }

    public async Task<ResponseBase> TestMe<TRequest>(TRequest request) where TRequest : RequestBase
    {
        var response = await _requestHandlerResolver
            .Resolve<TRequest>()
            .Handle(request);

        return response;
    }
}