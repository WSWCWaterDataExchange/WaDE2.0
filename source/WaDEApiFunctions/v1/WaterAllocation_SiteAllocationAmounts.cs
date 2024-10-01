using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WaDEApiFunctions;
using WesternStatesWater.WaDE.Managers.Api;

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
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmounts")] HttpRequestData req, ILogger log)
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
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Start index must be 0 or greater.");
                return badRequest;
            }

            if (recordCount < 1 || recordCount > 10000)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Record count must be between 1 and 10000");
                return badRequest;
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
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("At least one of the following filter parameters must be specified: siteUuid, beneficialUseCv, geometry, siteTypeCV, usgsCategoryNameCV, huc8, huc12, county, state");
                return badRequest;
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

            var jsonResult = req.CreateResponse(HttpStatusCode.OK);
            var json = JsonConvert.SerializeObject(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
            await jsonResult.WriteStringAsync(json);
            return jsonResult;
        }

        [Function("WaterAllocation_SiteAllocationAmountsDigest_v1")]
        public async Task<HttpResponseData> Digest([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmountsDigest")] HttpRequestData req, ILogger log)
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
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Start index must be 0 or greater.");
                return badRequest;
            }

            if (recordCount < 1 || recordCount > 10000)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Record count must be between 1 and 10000");
                return badRequest;
            }

            if (string.IsNullOrWhiteSpace(organizationUUID) &&
                string.IsNullOrWhiteSpace(beneficialUseCv) &&
                string.IsNullOrWhiteSpace(geometry) &&
                string.IsNullOrWhiteSpace(siteTypeCV) &&
                string.IsNullOrWhiteSpace(usgsCategoryNameCV))
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("At least one of the following filter parameters must be specified: organizationUUID, beneficialUseCv, geometry, siteTypeCV, usgsCategoryNameCV");
                return badRequest;
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

            var jsonResult = req.CreateResponse(HttpStatusCode.OK);
            var json = JsonConvert.SerializeObject(siteAllocationAmounts, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
            await jsonResult.WriteStringAsync(json);
            return jsonResult;
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
