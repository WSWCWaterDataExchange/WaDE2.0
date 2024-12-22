using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Integration.Tests.WaterResources;

[TestClass]
public class AggregatedAmountsIntegrationTests : IntegrationTestsBase
{
    private IAggregatedAmountsManager _manager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _manager = Services.GetRequiredService<IAggregatedAmountsManager>();
    }

    [TestMethod]
    public void SmokeTest()
    {
        _manager.Should().NotBeNull();
    }
}