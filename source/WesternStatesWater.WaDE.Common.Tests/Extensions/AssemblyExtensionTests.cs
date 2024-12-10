using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Common.Extensions;

namespace WesternStatesWater.WaDE.Common.Tests.Extensions;

[TestClass]
public class AssemblyExtensionTests
{
    [TestMethod]
    public async Task RegisterRequestHandlers_ShouldRegisterAllHandlersInTheAssembly()
    {
        var serviceCollection = new ServiceCollection();

        Assembly.GetExecutingAssembly().RegisterRequestHandlers(serviceCollection);

        serviceCollection.Count.Should().Be(1);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var response = await serviceProvider
            .GetRequiredService<IRequestHandler<TestRequest, TestResponse>>()
            .Handle(new TestRequest());

        response.Should().BeOfType<TestResponse>();
    }
}

file class TestRequest : RequestBase;

file class TestResponse : ResponseBase;

// ReSharper disable once UnusedType.Local It's used in the test above, but ReSharper doesn't know.
file class TestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request) => Task.FromResult(new TestResponse());
}