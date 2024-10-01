using System;
using Microsoft.Extensions.Hosting;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Managers.Import;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaDEImportFunctions;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.UseMiddleware<HttpContextAccessorMiddleware>();
    })
    .ConfigureAppConfiguration((_, configBuilder) =>
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        configBuilder
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"settings.{environment}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"settings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    })
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        services.AddHttpContextAccessor();

        services.AddSingleton(new BlobContainerClient(config.GetConnectionString("AzureStorage"), "normalizedimports"));
        services.AddSingleton(new BlobServiceClient(config.GetConnectionString("AzureStorage")));

        services.AddSingleton(config);
        services.AddTransient<ManagerImport.IWaterAllocationManager, WaterAllocationManager>();
        services.AddTransient<ManagerImport.IExcelFileConversionManager, ExcelFileConversionManager>();
        services.AddTransient<ManagerImport.IFlattenManager, FlattenManager>();
        services.AddTransient<AccessorImport.IDataIngestionAccessor, DataIngestionAccessor>();
        services.AddTransient<AccessorImport.IDataIngestionFileAccessor, DataIngestionFileAccessor>();
        services.AddTransient<AccessorImport.IBlobFileAccessor, BlobFileAccessor>();
    })
    .Build();

host.Run();