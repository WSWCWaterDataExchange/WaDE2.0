using Telerik.JustMock;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers;

public abstract class HandlerTestsBase
{
    protected IAggregatedAmountsAccessor AggregatedAmountsAccessorMock { get; } =
        Mock.Create<IAggregatedAmountsAccessor>(Behavior.Strict);
}