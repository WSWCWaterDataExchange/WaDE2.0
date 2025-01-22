using System.Transactions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telerik.JustMock;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Engines;
using WesternStatesWater.WaDE.Engines.Handlers;
using WesternStatesWater.WaDE.Managers.Api;
using WesternStatesWater.WaDE.Managers.Api.Handlers;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Utilities;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using AccessorExt = WesternStatesWater.WaDE.Accessors.Extensions;
using EngineApi = WesternStatesWater.WaDE.Engines.Contracts;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;
using ManagerExt = WesternStatesWater.WaDE.Managers.Api.Extensions;
using EngineExt = WesternStatesWater.WaDE.Engines.Extensions;

namespace WesternStatesWater.WaDE.Integration.Tests;

[TestClass]
public abstract class IntegrationTestsBase
{
    private TransactionScope _transactionScopeFixture = null!;

    protected IServiceProvider Services { get; private set; } = null!;
    protected static readonly IHttpContextAccessor _httpContextAccessor = Mock.Create<IHttpContextAccessor>();
    protected string SwaggerHostName = "https://proxy.example.com";
    protected string ApiHostName = "https://proxy.example.com/api";

    [TestInitialize]
    public void BaseTestInitialize()
    {
        Services = CreateServiceProvider();
        _transactionScopeFixture = CreateTransactionScope();
        
        // Setup Formatting Engine required configurations.
        Environment.SetEnvironmentVariable("OpenApi__HostNames", SwaggerHostName);
        Environment.SetEnvironmentVariable("OgcApi__Host", ApiHostName);
        Environment.SetEnvironmentVariable("OgcApi__Title", "WaDE Tests");
        Environment.SetEnvironmentVariable("OgcApi__Description", "WaDE Test Description");
    }

    /// <summary>
    /// Mocks the HttpContext to simulate a request to the specified path.
    /// </summary>
    /// <param name="path">Relative Url path. For example: /collections/sites/items</param>
    protected void MockRequestPath(string path)
    {
        path.Should().StartWith("/", $"{nameof(MockRequestPath)} should be called with a path starting with a /");
        var uri = new Uri($"{ApiHostName}{path}");
        var httpContext = new DefaultHttpContext();
#if DEBUG
        httpContext.Request.Scheme = uri.Scheme;
        httpContext.Request.Host = new HostString(uri.Host);
        httpContext.Request.Path = uri.AbsolutePath;
        httpContext.Request.QueryString = new QueryString(uri.Query);
#else
        httpContext.Request.Headers["X-WaDE-OriginalUrl"] = uri.AbsoluteUri;
#endif
        Mock.Arrange(() => _httpContextAccessor.HttpContext)
            .Returns(httpContext);
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        // Managers
        services.AddTransient<ManagerApi.IMetadataManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IWaterResourceManager, WaterResourceManager>();

        // Engines
        services.AddTransient<EngineApi.IValidationEngine, ValidationEngine>();
        services.AddTransient<EngineApi.IFormattingEngine, FormattingEngine>();

        // Accessors
        services.AddTransient<AccessorApi.IAggregatedAmountsAccessor, AggregratedAmountsAccessor>();
        services.AddTransient<AccessorApi.IRegulatoryOverlayAccessor, RegulatoryOverlayAccessor>();
        services.AddTransient<AccessorApi.ISiteVariableAmountsAccessor, SiteVariableAmountsAccessor>();
        services.AddTransient<AccessorApi.IWaterAllocationAccessor, WaterAllocationAccessor>();
        services.AddTransient<AccessorApi.ISiteAccessor, SiteAccessor>();

        // Handler resolvers
        services.AddScoped<
            IManagerRequestHandlerResolver,
            Managers.Api.Handlers.RequestHandlerResolver
        >();

        services.AddScoped<
            IEngineRequestHandlerResolver,
            Engines.Handlers.RequestHandlerResolver
        >();

        services.AddScoped<
            IAccessorRequestHandlerResolver,
            Accessors.Handlers.RequestHandlerResolver
        >();

        // Handlers
        ManagerExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);
        EngineExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);
        AccessorExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);

        // Utilities, config, misc.
        services.AddScoped<IConfiguration>(_ => Configuration.GetConfiguration());
        services.AddScoped<IHttpContextAccessor>(_ => _httpContextAccessor);
        services.AddLogging(config => config.AddConsole());
        services.AddTransient<IContextUtility, ContextUtility>();

        return services.BuildServiceProvider();
    }

    private static TransactionScope CreateTransactionScope()
    {
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.MaximumTimeout
        };

        return new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled
        );
    }

    [TestCleanup]
    public void BaseTestCleanup()
    {
        _transactionScopeFixture.Dispose();
    }
}