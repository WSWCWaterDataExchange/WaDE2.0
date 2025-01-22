using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Common.Contexts;

namespace WesternStatesWater.WaDE.Utilities.Tests.Context;

[TestClass]
public class ContextUtilityTests
{
    private readonly IHttpContextAccessor _httpContextAccessorMock = Mock.Create<IHttpContextAccessor>();

#if DEBUG
    [TestMethod]
    public void BuildContext_ApiContext_ShouldSetContextFromHttpContext()
    {
        var requestContext = new DefaultHttpContext();
        requestContext.Request.Scheme = "https";
        requestContext.Request.Host = new HostString("example.com");
        requestContext.Request.Path = "/api/foo";
        requestContext.Request.QueryString = new QueryString("?bar=baz");

        _httpContextAccessorMock.Arrange(mock => mock.HttpContext)
            .Returns(requestContext);

        var utility = new ContextUtility(_httpContextAccessorMock);

        utility.GetContext().Should().BeOfType<ApiContext>();
    }
#else
    [TestMethod]
    public void BuildContext_ApiContext_ShouldSetContextFromHeader()
    {
        var requestContext = new DefaultHttpContext();
        requestContext.Request.Scheme = "https";
        requestContext.Request.Host = new HostString("example.com");
        requestContext.Request.Path = "/api/v1/foo";
        requestContext.Request.QueryString = new QueryString("?bar=baz");
        requestContext.Request.Headers.Add("X-WaDE-OriginalUrl", "https://proxy.example.com/v2/foo?bar=baz");

        _httpContextAccessorMock.Arrange(mock => mock.HttpContext)
            .Returns(requestContext);

        var utility = new ContextUtility(_httpContextAccessorMock);

        var context = utility.GetContext();
        context.Should().BeOfType<ApiContext>();
        (context as ApiContext)!.RequestUri.Host.Should().Be("proxy.example.com");
    }
#endif
}