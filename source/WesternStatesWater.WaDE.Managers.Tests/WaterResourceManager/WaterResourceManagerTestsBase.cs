using Telerik.JustMock;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Tests.WaterResourceManager;

public abstract class WaterResourceManagerTestsBase
{
    protected IAggregatedAmountsAccessor AggregatedAmountsAccessorMock { get; } =
        Mock.Create<IAggregatedAmountsAccessor>(Behavior.Strict);

    protected IRegulatoryOverlayAccessor RegulatoryOverlayAccessorMock { get; }
        = Mock.Create<IRegulatoryOverlayAccessor>(Behavior.Strict);

    protected ISiteVariableAmountsAccessor SiteVariableAmountsAccessorMock { get; } =
        Mock.Create<ISiteVariableAmountsAccessor>(Behavior.Strict);

    protected IWaterAllocationAccessor WaterAllocationAccessorMock { get; } =
        Mock.Create<IWaterAllocationAccessor>(Behavior.Strict);

    internal Api.WaterResourceManager CreateWaterResourceManager()
    {
        return new Api.WaterResourceManager(
            AggregatedAmountsAccessorMock,
            RegulatoryOverlayAccessorMock,
            SiteVariableAmountsAccessorMock,
            WaterAllocationAccessorMock
        );
    }
}