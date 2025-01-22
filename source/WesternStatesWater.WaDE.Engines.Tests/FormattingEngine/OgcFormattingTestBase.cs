using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WesternStatesWater.WaDE.Common.Contexts;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

public class OgcFormattingTestBase
{
    protected readonly IContextUtility _contextUtilityMock = Mock.Create<IContextUtility>();
    protected string SwaggerHostName = "https://proxy.example.com";
    protected string ApiHostName = "https://proxy.example.com/api";
    
    [TestInitialize]
    public void OgcFormattingTestBaseInitialize()
    {
        Environment.SetEnvironmentVariable("OpenApi__HostNames", SwaggerHostName);
        Environment.SetEnvironmentVariable("OgcApi__Host", ApiHostName);
        Environment.SetEnvironmentVariable("OgcApi__Title", "WaDE Tests");
        Environment.SetEnvironmentVariable("OgcApi__Description", "WaDE Test Description");
    }
    
    protected void MockApiContextRequest(string path)
    {
        path.Should().StartWith("/", "MockApiContextRequest should be called with a path starting with a /");
        Mock.Arrange(() => _contextUtilityMock.GetRequiredContext<ApiContext>())
            .Returns(new ApiContext
            {
                RequestUri = new Uri($"{ApiHostName}{path}")
            });
    }
}