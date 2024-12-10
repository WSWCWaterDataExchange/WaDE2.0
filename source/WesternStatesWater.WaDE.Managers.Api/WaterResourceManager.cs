namespace WesternStatesWater.WaDE.Managers.Api;

internal partial class WaterResourceManager : ManagerBase
{
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
    ) : base(requestHandlerResolver)
    {
        _aggregratedAmountsAccessor = aggregratedAmountsAccessor;
        _regulatoryOverlayAccessor = regulatoryOverlayAccessor;
        _siteVariableAmountsAccessor = siteVariableAmountsAccessor;
        _waterAllocationAccessor = waterAllocationAccessor;
    }
}