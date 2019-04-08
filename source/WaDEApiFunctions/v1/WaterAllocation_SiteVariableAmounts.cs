using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WesternStatesWater.WaDE.Contracts.Api;
using Newtonsoft.Json.Serialization;

namespace WaDEApiFunctions.v1
{
    public class WaterAllocation_SiteVariableAmounts
    {
        public WaterAllocation_SiteVariableAmounts(IWaterAllocationManager waterAllocationManager)
        {
            WaterAllocationManager = waterAllocationManager;
        }

        private IWaterAllocationManager WaterAllocationManager { get; set; }

        [FunctionName("WaterAllocation_SiteVariableAmounts_v1")]
        public Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/SiteVariableAmounts")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_SiteVariableAmounts)}");

            var variableSpecificCV = req.Query["VariableSpecificCV"];
            var siteUuid = req.Query["SiteUUID"];

            if (string.IsNullOrWhiteSpace(variableSpecificCV) && string.IsNullOrWhiteSpace(siteUuid))
            {
                return Task.FromResult((IActionResult)new BadRequestObjectResult("Either VariableSpecificCV or SiteUUID must be specified"));
            }

            return Task.FromResult((IActionResult)new JsonResult(new string[0], new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() }));
        }
    }
}
