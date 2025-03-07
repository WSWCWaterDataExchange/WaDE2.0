using Telerik.JustMock;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers;

public abstract class HandlerTestsBase
{
    protected IAggregatedAmountsAccessor AggregatedAmountsAccessorMock { get; } =
        Mock.Create<IAggregatedAmountsAccessor>(Behavior.Strict);

    protected IRegulatoryOverlayAccessor RegulatoryOverlayAccessorMock { get; } =
        Mock.Create<IRegulatoryOverlayAccessor>(Behavior.Strict);

    protected ISiteVariableAmountsAccessor SiteVariableAmountsAccessorMock { get; } =
        Mock.Create<ISiteVariableAmountsAccessor>(Behavior.Strict);

    protected IWaterAllocationAccessor WaterAllocationAccessorMock { get; } =
        Mock.Create<IWaterAllocationAccessor>(Behavior.Strict);
}