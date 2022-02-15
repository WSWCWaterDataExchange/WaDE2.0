using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Managers.Import;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;

[assembly: WebJobsStartup(typeof(WaDEImportFunctions.Startup))]

namespace WaDEImportFunctions
{

    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton(new BlobContainerClient(config.GetConnectionString("AzureStorage"), "normalizedimports"));
            builder.Services.AddSingleton(new BlobServiceClient(config.GetConnectionString("AzureStorage")));

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddTransient<ManagerImport.IWaterAllocationManager, WaterAllocationManager>();
            builder.Services.AddTransient<ManagerImport.IExcelFileConversionManager, ExcelFileConversionManager>();
            builder.Services.AddTransient<ManagerImport.IFlattenManager, FlattenManager>();
            builder.Services.AddTransient<AccessorImport.IDataIngestionAccessor, DataIngestionAccessor>();
            builder.Services.AddTransient<AccessorImport.IDataIngestionFileAccessor, DataIngestionFileAccessor>();
            builder.Services.AddTransient<AccessorImport.IBlobFileAccessor, BlobFileAccessor>();
        }
    }
}
