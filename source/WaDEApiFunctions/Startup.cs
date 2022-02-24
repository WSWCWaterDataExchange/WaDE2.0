using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Managers.Api;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

[assembly: WebJobsStartup(typeof(WaDEApiFunctions.Startup))]
[assembly: InternalsVisibleTo("WesternStatesWater.WaDE.Clients.Tests")]

namespace WaDEApiFunctions
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

            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddTransient<ManagerApi.IWaterAllocationManager, WaterAllocationManager>();
            builder.Services.AddTransient<AccessorApi.IWaterAllocationAccessor, WaterAllocationAccessor>();

            builder.Services.AddTransient<ManagerApi.IAggregatedAmountsManager, AggregratedAmountsManager>();
            builder.Services.AddTransient<AccessorApi.IAggregatedAmountsAccessor, AggregratedAmountsAccessor>();

            builder.Services.AddTransient<ManagerApi.ISiteVariableAmountsManager, SiteVariableAmountsManager>();
            builder.Services.AddTransient<AccessorApi.ISiteVariableAmountsAccessor, SiteVariableAmountsAccessor>();

            builder.Services.AddTransient<ManagerApi.IRegulatoryOverlayManager, RegulatoryOverlayManager>();
            builder.Services.AddTransient<AccessorApi.IRegulatoryOverlayAccessor, RegulatoryOverlayAccessor>();
        }
    }
}
