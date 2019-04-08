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
    public class WaterAllocation_SiteAllocationAmounts
    {
        public WaterAllocation_SiteAllocationAmounts(IWaterAllocationManager waterAllocationManager)
        {
            WaterAllocationManager = waterAllocationManager;
        }

        private IWaterAllocationManager WaterAllocationManager { get; set; }

        [FunctionName("WaterAllocation_SiteAllocationAmounts_v1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/SiteAllocationAmounts")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)}");

            var variableSpecificCV = req.Query["VariableSpecificCV"];
            var siteUuid = req.Query["SiteUUID"];
            var beneficialUse = req.Query["BeneficialUse"];
            var geometry = req.Query["Geometry"];

            if (string.IsNullOrWhiteSpace(variableSpecificCV) && string.IsNullOrWhiteSpace(siteUuid) && string.IsNullOrWhiteSpace(beneficialUse) && string.IsNullOrWhiteSpace(geometry))
            {
                return new BadRequestObjectResult("VariableSpecificCV, SiteUUID, BeneficialUse, or Geometry must be specified");
            }

            var siteAllocationAmounts = await WaterAllocationManager.GetSiteAllocationAmountsAsync(variableSpecificCV, siteUuid, beneficialUse, geometry);

            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }
    }
}
