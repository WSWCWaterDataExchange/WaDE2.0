using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Common.Extensions;
using WesternStatesWater.WaDE.Common.Tests.Helpers;

namespace WesternStatesWater.WaDE.Common.Tests.Extensions;

[TestClass]
public class AssemblyExtensionTests
{
    [TestMethod]
    public async Task RegisterRequestHandlers_ShouldRegisterAllHandlersInTheAssembly()
    {
        var serviceCollection = new ServiceCollection();

        Assembly.GetExecutingAssembly().RegisterRequestHandlers(serviceCollection);

        // From TestRequestHandler.cs and RequestHandlerResolverBaseTests.cs
        serviceCollection.Count.Should().Be(2);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var response = await serviceProvider
            .GetRequiredService<IRequestHandler<TestRequest, TestResponse>>()
            .Handle(new TestRequest());

        response.Should().BeOfType<TestResponse>();
    }
}