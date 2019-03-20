using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WesternStatesWater.WaDE.Accessors;
using WesternStatesWater.WaDE.Accessors.Contracts;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Contracts.Api;
using WesternStatesWater.WaDE.Managers;

[assembly: WebJobsStartup(typeof(WaDEApiFunctions.Startup))]

namespace WaDEApiFunctions
{

    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{Environment.UserName}.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<IConfiguration>(config);
            builder.Services.AddTransient<IWaterAllocationManager, WaterAllocationManager>();
            builder.Services.AddTransient<IWaterAllocationAccessor, WaterAllocationAccessor>();

        }
    }
}
