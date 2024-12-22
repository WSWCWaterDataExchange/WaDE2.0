using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Engines;
using WesternStatesWater.WaDE.Managers.Api;
using WesternStatesWater.WaDE.Managers.Api.Handlers;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
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

    [TestInitialize]
    public void BaseTestInitialize()
    {
        Services = CreateServiceProvider();
        _transactionScopeFixture = CreateTransactionScope();
    }

    private static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        // Managers
        services.AddTransient<ManagerApi.IAggregatedAmountsManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IRegulatoryOverlayManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.ISiteVariableAmountsManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IWaterAllocationManager, WaterResourceManager>();
        services.AddTransient<ManagerApi.IMetadataManager, WaterResourceManager>();

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
            Engines.RequestHandlerResolver
        >();

        // Handlers
        ManagerExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);
        EngineExt.ServiceCollectionExtensions.RegisterRequestHandlers(services);

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