using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Managers.Api;
using WesternStatesWater.WaDE.Managers.Api.Extensions;
using WesternStatesWater.WaDE.Managers.Api.Handlers;

namespace WesternStatesWater.WaDE.Managers.Tests;

[TestClass]
public class ManagerBaseTests
{
    private TestManager _manager = null;

    [TestInitialize]
    public void TestInitialize()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped<IRequestHandlerResolver, RequestHandlerResolver>()
            .RegisterRequestHandlers()
            .BuildServiceProvider();

        _manager = new TestManager(serviceProvider.GetRequiredService<IRequestHandlerResolver>());
    }

    [TestMethod]
    public async Task ExecuteAsync_SmokeTest()
    {
        var request = new SearchOverlaysRequest();
        var response = await _manager.ExecuteAsync<SearchOverlaysRequest, SearchOverlaysResponse>(request);

        response.Should().NotBeNull();
        response.Should().BeOfType<SearchOverlaysResponse>();
    }

    private class TestManager(IRequestHandlerResolver resolver) : ManagerBase(resolver);
}