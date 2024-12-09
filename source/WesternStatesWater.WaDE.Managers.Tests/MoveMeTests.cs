using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Managers.Api;

namespace WesternStatesWater.WaDE.Managers.Tests;

[TestClass]
public class MoveMeTests
{
    [TestMethod]
    public async Task SmokeTest()
    {
        var services = new ServiceCollection().RegisterHandlerFactory().BuildServiceProvider();

        var resolver = services.GetRequiredService<IRequestHandlerResolver>();

        var result = await resolver
            .Resolve<SearchOverlayRequest>()
            .Handle(new SearchOverlayRequest());
    }
}