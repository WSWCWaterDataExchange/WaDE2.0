using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WesternStatesWater.Shared.DataContracts;
using WesternStatesWater.Shared.Errors;
using WesternStatesWater.Shared.Resolver;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Api;
using WesternStatesWater.WaDE.Managers.Api.Handlers;

namespace WesternStatesWater.WaDE.Managers.Tests
{
    [TestClass]
    public class ManagerBaseTests
    {
        private TestManager _manager;

        private readonly IRequestHandler<TestRequest, TestResponse> _requestHandlerMock =
            Mock.Create<IRequestHandler<TestRequest, TestResponse>>(Behavior.Strict);

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(config => config.AddConsole())
                .AddTransient<IRequestHandlerResolver, RequestHandlerResolver>()
                .AddTransient(_ => _requestHandlerMock)
                .BuildServiceProvider();

            var resolver = serviceProvider.GetRequiredService<IRequestHandlerResolver>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<TestManager>();

            _manager = new TestManager(logger, resolver);
        }

        [TestMethod]
        public async Task ExecuteAsync_ShouldReturnHandlerResult()
        {
            var request = new TestRequest { Id = 42 };

            Mock.Arrange(() => _requestHandlerMock.Handle(request))
                .Returns(Task.FromResult(new TestResponse { Message = "Success" }));

            var response = await _manager.ExecuteAsync<TestRequest, TestResponse>(request);

            response.Message.Should().Be("Success");
        }

        [TestMethod]
        public async Task ExecuteAsync_UnhandledException_ShouldReturnInternalError()
        {
            var request = new TestRequest { Id = 42 };

            Mock.Arrange(() => _requestHandlerMock.Handle(request))
                .Throws(new Exception("Whoopsie daisy"));

            var response = await _manager.ExecuteAsync<TestRequest, TestResponse>(request);

            response.Should().NotBeNull();
            response.Should().BeOfType<TestResponse>();
            response.Error.Should().NotBeNull();
            response.Error.Should().BeOfType<InternalError>();
        }

        [TestMethod]
        public async Task ExecuteAsync_InputValidationFails_ShouldReturnValidationError()
        {
            var request = new TestRequest { Id = 43 };
            var response = await _manager.ExecuteAsync<TestRequest, TestResponse>(request);

            response.Should().NotBeNull();
            response.Should().BeOfType<TestResponse>();
            response.Error.Should().NotBeNull();
            response.Error.Should().BeOfType<ValidationError>();

            var error = (ValidationError)response.Error!;
            error.Errors.Keys.Count.Should().Be(1);
            error.Errors["Id"].Should().Contain("'Id' must be less than '43'.");
        }

        private class TestManager(ILogger<TestManager> logger, IRequestHandlerResolver resolver)
            : ManagerBase(logger, resolver);
    }
}

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public abstract class TestRequestBase : RequestBase;

    public class TestRequest : TestRequestBase
    {
        public long Id { get; init; }
    }

    public sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Id).LessThan(43).GreaterThan(41);
        }
    }

    public class TestResponse : ResponseBase
    {
        public string Message { get; init; }
    }
}