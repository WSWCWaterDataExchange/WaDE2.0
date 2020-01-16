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
            log.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)} Run");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SiteAllocationAmountsRequestBody>(requestBody);

            var siteUuid = ((string)req.Query["SiteUUID"]) ?? data?.siteUUID;
            var siteTypeCV = ((string)req.Query["SiteTypeCV"]) ?? data?.siteTypeCV;
            var beneficialUseCv = ((string)req.Query["BeneficialUseCV"]) ?? data?.beneficialUseCV;
            var usgsCategoryNameCV = ((string)req.Query["USGSCategoryNameCV"]) ?? data?.USGSCategoryNameCV;
            var geometry = ((string)req.Query["SearchGeometry"]) ?? data?.searchGeometry;
            var startPriorityDate = ParseDate(((string)req.Query["StartPriorityDate"]) ?? data?.startPriorityDate);
            var endPriorityDate = ParseDate(((string)req.Query["EndPriorityDate"]) ?? data?.endPriorityDate);
            var huc8 = ((string)req.Query["HUC8"]) ?? data?.huc8;
            var huc12 = ((string)req.Query["HUC12"]) ?? data?.huc12;
            var county = ((string)req.Query["County"]) ?? data?.county;
            var state = ((string)req.Query["State"]) ?? data?.state;
            var startIndex = ParseInt(((string)req.Query["StartIndex"]) ?? data?.startIndex) ?? 0;
            var recordCount = ParseInt(((string)req.Query["RecordCount"]) ?? data?.recordCount) ?? 1000;

            if (startIndex < 0)
            {
                return new BadRequestObjectResult("Start index must be 0 or greater.");
            }

            if (recordCount < 1 || recordCount > 10000)
            {
                return new BadRequestObjectResult("Record count must be between 1 and 10000");
            }

            if (string.IsNullOrWhiteSpace(siteUuid) && 
                string.IsNullOrWhiteSpace(beneficialUseCv) && 
                string.IsNullOrWhiteSpace(geometry) && 
                string.IsNullOrWhiteSpace(siteTypeCV) && 
                string.IsNullOrWhiteSpace(usgsCategoryNameCV) && 
                string.IsNullOrWhiteSpace(huc8) &&
                string.IsNullOrWhiteSpace(huc12) &&
                string.IsNullOrWhiteSpace(county) &&
                string.IsNullOrWhiteSpace(state))
            {
                return new BadRequestObjectResult("At least one filter parameter must be specified");
            }

            var siteAllocationAmounts = await WaterAllocationManager.GetSiteAllocationAmountsAsync(new SiteAllocationAmountsFilters
            {
                BeneficialUseCv = beneficialUseCv,
                Geometry = geometry,
                SiteTypeCV = siteTypeCV,
                SiteUuid = siteUuid,
                UsgsCategoryNameCv = usgsCategoryNameCV,
                StartPriorityDate = startPriorityDate,
                EndPriorityDate = endPriorityDate,
                HUC8 = huc8,
                HUC12 = huc12,
                County = county,
                State = state
            }, startIndex, recordCount);

            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }

        [FunctionName("WaterAllocation_SiteAllocationAmountsDigest_v1")]
        public async Task<IActionResult> Digest([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmountsDigest")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)} Digest");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SiteAllocationAmountsDigestRequestBody>(requestBody);
            
            var siteTypeCV = ((string)req.Query["SiteTypeCV"]) ?? data?.siteTypeCV;
            var beneficialUseCv = ((string)req.Query["BeneficialUseCV"]) ?? data?.beneficialUseCV;
            var usgsCategoryNameCV = ((string)req.Query["USGSCategoryNameCV"]) ?? data?.USGSCategoryNameCV;
            var geometry = ((string)req.Query["SearchGeometry"]) ?? data?.searchGeometry;
            var startPriorityDate = ParseDate(((string)req.Query["StartPriorityDate"]) ?? data?.startPriorityDate);
            var endPriorityDate = ParseDate(((string)req.Query["EndPriorityDate"]) ?? data?.endPriorityDate);
            var organizationUUID = ((string)req.Query["OrganizationUUID"]) ?? data?.organizationUUID;
            
            var startIndex = ParseInt(((string)req.Query["StartIndex"]) ?? data?.startIndex) ?? 0;
            var recordCount = ParseInt(((string)req.Query["RecordCount"]) ?? data?.recordCount) ?? 1000;

            if (startIndex < 0)
            {
                return new BadRequestObjectResult("Start index must be 0 or greater.");
            }

            if (recordCount < 1 || recordCount > 10000)
            {
                return new BadRequestObjectResult("Record count must be between 1 and 10000");
            }

            if (string.IsNullOrWhiteSpace(organizationUUID) && 
                string.IsNullOrWhiteSpace(beneficialUseCv) && 
                string.IsNullOrWhiteSpace(geometry) && 
                string.IsNullOrWhiteSpace(siteTypeCV) && 
                string.IsNullOrWhiteSpace(usgsCategoryNameCV))
            {
                return new BadRequestObjectResult("At least one filter parameter must be specified");
            }

            var siteAllocationAmounts = await WaterAllocationManager.GetSiteAllocationAmountsDigestAsync(new SiteAllocationAmountsDigestFilters
            {
                BeneficialUseCv = beneficialUseCv,
                Geometry = geometry,
                SiteTypeCV = siteTypeCV,
                UsgsCategoryNameCv = usgsCategoryNameCV,
                StartPriorityDate = startPriorityDate,
                EndPriorityDate = endPriorityDate,      
                OrganizationUUID = organizationUUID
            }, startIndex, recordCount);

            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }

        private static DateTime? ParseDate(string value)
        {
            return DateTime.TryParse(value, out var date) ? date : (DateTime?)null;
        }

        private static int? ParseInt(string value)
        {
            return int.TryParse(value, out var date) ? date : (int?)null;
        }

        private class SiteAllocationAmountsRequestBody
        {
            public string startPriorityDate { get; set; }
            public string endPriorityDate { get; set; }
            public string siteUUID { get; set; }
            public string siteTypeCV { get; set; }
            public string USGSCategoryNameCV { get; set; }
            public string beneficialUseCV { get; set; }
            public string searchGeometry { get; set; }
            public string huc8 { get; set; }
            public string huc12 { get; set; }
            public string county { get; set; }
            public string state { get; set; }
            public string startIndex { get; set; }
            public string recordCount { get; set; }
        }

        private class SiteAllocationAmountsDigestRequestBody
        {
            public string startPriorityDate { get; set; }
            public string endPriorityDate { get; set; }
            public string organizationUUID { get; set; }
            public string siteTypeCV { get; set; }
            public string USGSCategoryNameCV { get; set; }
            public string beneficialUseCV { get; set; }
            public string searchGeometry { get; set; }
            public string startIndex { get; set; }
            public string recordCount { get; set; }
        }
    }
}
