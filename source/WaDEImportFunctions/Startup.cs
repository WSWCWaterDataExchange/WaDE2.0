using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Contracts.Import;
using WesternStatesWater.WaDE.Managers;

[assembly: WebJobsStartup(typeof(WaDEImportFunctions.Startup))]

namespace WaDEImportFunctions
{

    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddTransient<IWaterAllocationManager, WaterAllocationManager>();
            builder.Services.AddTransient<IWaterAllocationAccessor, WaterAllocationAccessor>();
            builder.Services.AddTransient<IWaterAllocationFileAccessor, WaterAllocationFileAccessor>();
        }
    }
}
