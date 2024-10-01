using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using Microsoft.Azure.Functions.Worker;

namespace WaDEApiFunctions.v1
{
    public class WaterAllocation_SiteAllocationAmounts
    {
        public WaterAllocation_SiteAllocationAmounts(IWaterAllocationManager waterAllocationManager)
        {
            WaterAllocationManager = waterAllocationManager;
        }

        private IWaterAllocationManager WaterAllocationManager { get; set; }
        
        [Function("WaterAllocation_SiteAllocationAmounts_v1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmounts")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)} Run");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SiteAllocationAmountsRequestBody>(requestBody);

            var siteUuid = req.GetQueryString("SiteUUID") ?? data?.siteUUID;
            var siteTypeCV = req.GetQueryString("SiteTypeCV") ?? data?.siteTypeCV;
            var beneficialUseCv = req.GetQueryString("BeneficialUseCV") ?? data?.beneficialUseCV;
            var usgsCategoryNameCV = req.GetQueryString("USGSCategoryNameCV") ?? data?.USGSCategoryNameCV;
            var geometry = req.GetQueryString("SearchGeometry") ?? data?.searchGeometry;
            var startPriorityDate = RequestDataParser.ParseDate(req.GetQueryString("StartPriorityDate") ?? data?.startPriorityDate);
            var endPriorityDate = RequestDataParser.ParseDate(req.GetQueryString("EndPriorityDate") ?? data?.endPriorityDate);
            var startDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("StartPublicationDate") ?? data?.startPublicationDate);
            var endDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("EndPublicationDate") ?? data?.endPublicationDate);
            var huc8 = req.GetQueryString("HUC8") ?? data?.huc8;
            var huc12 = req.GetQueryString("HUC12") ?? data?.huc12;
            var county = req.GetQueryString("County") ?? data?.county;
            var state = req.GetQueryString("State") ?? data?.state;
            var startIndex = RequestDataParser.ParseInt(req.GetQueryString("StartIndex") ?? data?.startIndex) ?? 0;
            var recordCount = RequestDataParser.ParseInt(req.GetQueryString("RecordCount") ?? data?.recordCount) ?? 1000;
            var geoFormat = RequestDataParser.ParseGeometryFormat(req.GetQueryString("geoFormat")) ?? GeometryFormat.Wkt;

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
                return new BadRequestObjectResult("At least one of the following filter parameters must be specified: siteUuid, beneficialUseCv, geometry, siteTypeCV, usgsCategoryNameCV, huc8, huc12, county, state");
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
                StartDataPublicationDate = startDataPublicationDate,
                EndDataPublicationDate = endDataPublicationDate,
                HUC8 = huc8,
                HUC12 = huc12,
                County = county,
                State = state
            }, startIndex, recordCount, geoFormat);

            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }

        [Function("WaterAllocation_SiteAllocationAmountsDigest_v1")]
        public async Task<IActionResult> Digest([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmountsDigest")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)} Digest");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SiteAllocationAmountsDigestRequestBody>(requestBody);

            var siteTypeCV = req.GetQueryString("SiteTypeCV") ?? data?.siteTypeCV;
            var beneficialUseCv = req.GetQueryString("BeneficialUseCV") ?? data?.beneficialUseCV;
            var usgsCategoryNameCV = req.GetQueryString("USGSCategoryNameCV") ?? data?.USGSCategoryNameCV;
            var geometry = req.GetQueryString("SearchGeometry") ?? data?.searchGeometry;
            var startPriorityDate = RequestDataParser.ParseDate(req.GetQueryString("StartPriorityDate") ?? data?.startPriorityDate);
            var endPriorityDate = RequestDataParser.ParseDate(req.GetQueryString("EndPriorityDate") ?? data?.endPriorityDate);
            var startDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("StartPublicationDate") ?? data?.startPublicationDate);
            var endDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("EndPublicationDate") ?? data?.endPublicationDate);
            var organizationUUID = req.GetQueryString("OrganizationUUID") ?? data?.organizationUUID;

            var startIndex = RequestDataParser.ParseInt(((string)req.Query["StartIndex"]) ?? data?.startIndex) ?? 0;
            var recordCount = RequestDataParser.ParseInt(((string)req.Query["RecordCount"]) ?? data?.recordCount) ?? 1000;

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
                return new BadRequestObjectResult("At least one of the following filter parameters must be specified: organizationUUID, beneficialUseCv, geometry, siteTypeCV, usgsCategoryNameCV");
            }

            var siteAllocationAmounts = await WaterAllocationManager.GetSiteAllocationAmountsDigestAsync(new SiteAllocationAmountsDigestFilters
            {
                BeneficialUseCv = beneficialUseCv,
                Geometry = geometry,
                SiteTypeCV = siteTypeCV,
                UsgsCategoryNameCv = usgsCategoryNameCV,
                StartPriorityDate = startPriorityDate,
                EndPriorityDate = endPriorityDate,
                StartDataPublicationDate = startDataPublicationDate,
                EndDataPublicationDate = endDataPublicationDate,
                OrganizationUUID = organizationUUID
            }, startIndex, recordCount);

            return new JsonResult(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
        }

        private sealed class SiteAllocationAmountsRequestBody
        {
            public string startPriorityDate { get; set; }
            public string endPriorityDate { get; set; }
            public string startPublicationDate { get; set; }
            public string endPublicationDate { get; set; }
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

        private sealed class SiteAllocationAmountsDigestRequestBody
        {
            public string startPriorityDate { get; set; }
            public string endPriorityDate { get; set; }
            public string startPublicationDate { get; set; }
            public string endPublicationDate { get; set; }
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
