using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Managers.Api;
using WesternStatesWater.WaDE.Managers.Api.Extensions;

namespace WesternStatesWater.WaDE.Managers.Tests;

[TestClass]
public class ManagerBaseTests
{
    [TestMethod]
    public async Task SmokeTest()
    {
        var services = new ServiceCollection().RegisterRequestHandlerResolver().BuildServiceProvider();

        var resolver = services.GetRequiredService<IRequestHandlerResolver>();

        var result = await resolver
            .Resolve<SearchOverlaysRequest>()
            .Handle(new SearchOverlaysRequest());
    }
}