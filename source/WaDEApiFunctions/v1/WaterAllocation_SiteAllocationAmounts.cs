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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmounts")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SiteAllocationAmountsRequestBody>(requestBody);

            var variableSpecificCV = ((string)req.Query["VariableSpecificCV"]) ?? data?.variableSpecificCV;
            var siteUuid = ((string)req.Query["SiteUUID"]) ?? data?.siteUUID;
            var beneficialUse = ((string)req.Query["BeneficialUse"]) ?? data?.beneficialUse;
            var geometry = ((string)req.Query["Geometry"]) ?? data?.geometry;

            if (string.IsNullOrWhiteSpace(variableSpecificCV) && string.IsNullOrWhiteSpace(siteUuid) && string.IsNullOrWhiteSpace(beneficialUse) && string.IsNullOrWhiteSpace(geometry))
            {
                return new BadRequestObjectResult("VariableSpecificCV, SiteUUID, BeneficialUse, or Geometry must be specified");
            }

            var siteAllocationAmounts = await WaterAllocationManager.GetSiteAllocationAmountsAsync(variableSpecificCV, siteUuid, beneficialUse, geometry);

            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }

        private class SiteAllocationAmountsRequestBody
        {
            public string variableSpecificCV { get; set; }
            public string siteUUID { get; set; }
            public string beneficialUse { get; set; }
            public string geometry { get; set; }
        }
    }
}
