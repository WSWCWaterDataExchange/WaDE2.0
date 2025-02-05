using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using WesternStatesWater.WaDE.Common.Contexts;
using WesternStatesWater.WaDE.Utilities;

namespace WesternStatesWater.WaDE.Engines.Tests.FormattingEngine;

public class OgcFormattingTestBase
{
    protected readonly IContextUtility _contextUtilityMock = Mock.Create<IContextUtility>();
    protected string SwaggerDoc = "https://proxy.example.com/swagger/ui";
    protected string SwaggerDescription = "https://proxy.example.com/swagger/swagger.json";
    protected string ApiHostName = "https://proxy.example.com/api";
    
    [TestInitialize]
    public void OgcFormattingTestBaseInitialize()
    {
        Environment.SetEnvironmentVariable("OgcApi__Host", ApiHostName);
        Environment.SetEnvironmentVariable("OgcApi__SwaggerDoc", SwaggerDoc);
        Environment.SetEnvironmentVariable("OgcApi__SwaggerDescription", SwaggerDescription);
        Environment.SetEnvironmentVariable("OgcApi__Title", "WaDE Tests");
        Environment.SetEnvironmentVariable("OgcApi__Description", "WaDE Test Description");
    }

    [TestCleanup]
    public void OgcFormattingTestBaseCleanup()
    {
        Environment.SetEnvironmentVariable("OgcApi__Host", null);
        Environment.SetEnvironmentVariable("OgcApi__SwaggerDocs", null);
        Environment.SetEnvironmentVariable("OgcApi__SwaggerDescription", null);
        Environment.SetEnvironmentVariable("OgcApi__Title", null);
        Environment.SetEnvironmentVariable("OgcApi__Description", null);
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