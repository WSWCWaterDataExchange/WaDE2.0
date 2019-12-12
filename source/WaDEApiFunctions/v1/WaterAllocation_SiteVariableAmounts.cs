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
    public class WaterAllocation_SiteVariableAmounts
    {
        public WaterAllocation_SiteVariableAmounts(ISiteVariableAmountsManager siteVariableAmountsManager)
        {
            SiteVariableAmountsManager = siteVariableAmountsManager;
        }

        private ISiteVariableAmountsManager SiteVariableAmountsManager { get; set; }

        [FunctionName("WaterAllocation_SiteVariableAmounts_v1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteVariableAmounts")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_AggregatedAmounts)}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AggregratedAmountsRequestBody>(requestBody);

            var siteUUID = ((string)req.Query["SiteUUID"]) ?? data?.siteUUID;
            var siteTypeCV = ((string)req.Query["SiteTypeCV"]) ?? data?.siteTypeCV;
            var variableCV = ((string)req.Query["VariableCV"]) ?? data?.variableCV;
            var variableSpecificCV = ((string)req.Query["VariableSpecificCV"]) ?? data?.variableCV;
            var beneficialUse = ((string)req.Query["BeneficialUseCV"]) ?? data?.beneficialUseCV;
            var usgsCategoryNameCV = ((string)req.Query["UsgsCategoryNameCV"]) ?? data?.usgsCategoryNameCV;
            var startDate = ParseDate(((string)req.Query["StartDate"]) ?? data?.startDate);
            var endDate = ParseDate(((string)req.Query["EndDate"]) ?? data?.endDate);
            var geometry = ((string)req.Query["SearchBoundary"]) ?? data?.searchBoundary;
            var huc8 = ((string)req.Query["HUC8"]) ?? data?.huc8;
            var huc12 = ((string)req.Query["HUC12"]) ?? data?.huc12;
            var county = ((string)req.Query["County"]) ?? data?.county;
            var state = ((string)req.Query["State"]) ?? data?.state;

            if (string.IsNullOrWhiteSpace(variableCV) && string.IsNullOrWhiteSpace(variableSpecificCV) && string.IsNullOrWhiteSpace(beneficialUse) && string.IsNullOrWhiteSpace(siteUUID) && string.IsNullOrWhiteSpace(geometry) && string.IsNullOrWhiteSpace(siteTypeCV) && string.IsNullOrWhiteSpace(usgsCategoryNameCV))
            {
                return new BadRequestObjectResult("At least one filter parameter must be specified");
            }

            var siteAllocationAmounts = await SiteVariableAmountsManager.GetSiteVariableAmountsAsync(new SiteVariableAmountsFilters
            {
                SiteUuid = siteUUID,
                SiteTypeCv = siteTypeCV,
                VariableCv = variableCV,
                VariableSpecificCv = variableSpecificCV,
                BeneficialUseCv = beneficialUse,
                UsgsCategoryNameCv = usgsCategoryNameCV,
                Geometry = geometry,
                TimeframeStartDate = startDate,
                TimeframeEndDate = endDate,
                HUC8 = huc8,
                HUC12 = huc12,
                County = county,
                State = state
            });
            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }

        private static DateTime? ParseDate(string value)
        {
            return DateTime.TryParse(value, out var date) ? date : (DateTime?)null;
        }

        private class AggregratedAmountsRequestBody
        {
            public string siteUUID { get; set; }
            public string siteTypeCV { get; set; }
            public string variableCV { get; set; }
            public string variableSpecificCV { get; set; }
            public string beneficialUseCV { get; set; }
            public string usgsCategoryNameCV { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string searchBoundary { get; set; }
            public string huc8 { get; set; }
            public string huc12 { get; set; }
            public string county { get; set; }
            public string state { get; set; }
        }
    }
}
