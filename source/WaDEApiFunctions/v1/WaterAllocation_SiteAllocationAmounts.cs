using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;

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

            var siteUuid = ((string)req.Query["SiteUUID"]) ?? data?.siteUUID;
            var beneficialUse = ((string)req.Query["BeneficialUse"]) ?? data?.beneficialUse;
            var geometry = ((string)req.Query["Geometry"]) ?? data?.geometry;
            var startPriorityDate = ParseDate(((string)req.Query["StartPriorityDate"]) ?? data?.startPriorityDate);
            var endPriorityDate = ParseDate(((string)req.Query["EndPriorityDate"]) ?? data?.endPriorityDate);

            if (string.IsNullOrWhiteSpace(siteUuid) && string.IsNullOrWhiteSpace(beneficialUse) && string.IsNullOrWhiteSpace(geometry))
            {
                return new BadRequestObjectResult("SiteUUID, BeneficialUse, or Geometry must be specified");
            }

            var siteAllocationAmounts = await WaterAllocationManager.GetSiteAllocationAmountsAsync(siteUuid, beneficialUse, geometry, startPriorityDate, endPriorityDate);

            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }

        private static DateTime? ParseDate(string value)
        {
            return DateTime.TryParse(value, out var date) ? date : (DateTime?)null;
        }

        private class SiteAllocationAmountsRequestBody
        {
            public string startPriorityDate { get; set; }
            public string endPriorityDate { get; set; }
            public string siteUUID { get; set; }
            public string beneficialUse { get; set; }
            public string geometry { get; set; }
        }
    }
}
