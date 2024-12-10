using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Common.Contracts;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers.Api.Handlers;

namespace WesternStatesWater.WaDE.Managers.Tests.Handlers
{
    [TestClass]
    public class RequestHandlerResolverTests
    {
        private readonly RequestHandlerResolver _resolver = new RequestHandlerResolver(
            new ServiceCollection().BuildServiceProvider()
        );

        [TestMethod]
        public void ValidateTypeNamespace_TypeIsInManagerContractNamespace_ShouldNotThrow()
        {
            var action = () => _resolver.ValidateTypeNamespace(typeof(RightNamespaceTestType));

            action.Should().NotThrow();
        }

        [TestMethod]
        public void ValidateTypeNamespace_TypeIsNotInManagerContractNamespace_ShouldThrow()
        {
            var action = () => _resolver.ValidateTypeNamespace(typeof(WrongNamespaceTestType));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage(
                    "Type WesternStatesWater.WaDE.Managers.Tests.Handlers.WrongNamespaceTestType " +
                    "is not a valid request type. Request types must be in the WesternStatesWater.WaDE.Contracts.Api namespace."
                );
        }
    }

    public class WrongNamespaceTestType : RequestBase;
}

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class RightNamespaceTestType : RequestBase;
}