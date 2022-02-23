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
    public class WaterAllocation_AggregatedAmounts
    {
        public WaterAllocation_AggregatedAmounts(IAggregatedAmountsManager aggregatedAmountsManager)
        {
            AggregatedAmountsManager = aggregatedAmountsManager;
        }

        private IAggregatedAmountsManager AggregatedAmountsManager { get; set; }

        [FunctionName("WaterAllocation_AggregatedAmounts_v1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/AggregatedAmounts")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Call to {nameof(WaterAllocation_AggregatedAmounts)}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AggregratedAmountsRequestBody>(requestBody);

            var variableCV = req.GetQueryString("VariableCV") ?? data?.variableCV;
            var variableSpecificCV = req.GetQueryString("VariableSpecificCV") ?? data?.variableCV;
            var beneficialUse = req.GetQueryString("BeneficialUseCV") ?? data?.beneficialUseCV;
            var startDate = ParseDate(req.GetQueryString("StartDate") ?? data?.startDate);
            var endDate = ParseDate(req.GetQueryString("EndDate") ?? data?.endDate);
            var reportingUnitUUID = req.GetQueryString("ReportingUnitUUID") ?? data?.reportingUnitUUID;
            var geometry = req.GetQueryString("SearchBoundary") ?? data?.searchBoundary;
            var reportingUnitTypeCV = req.GetQueryString("ReportingUnitTypeCV") ?? data?.reportingUnitTypeCV;
            var usgsCategoryNameCV = req.GetQueryString("UsgsCategoryNameCV") ?? data?.usgsCategoryNameCV;
            var state = req.GetQueryString("State") ?? data?.state;
            var startIndex = ParseInt(req.GetQueryString("StartIndex") ?? data?.startIndex) ?? 0;
            var recordCount = ParseInt(req.GetQueryString("RecordCount") ?? data?.recordCount) ?? 1000;
            var geoFormat = RequestDataParser.ParseGeometryFormat(req.GetQueryString("geoFormat")) ?? GeometryFormat.Wkt;

            if (startIndex < 0)
            {
                return new BadRequestObjectResult("Start index must be 0 or greater.");
            }

            if (recordCount < 1 || recordCount > 10000)
            {
                return new BadRequestObjectResult("Record count must be between 1 and 10000");
            }

            if (string.IsNullOrWhiteSpace(variableCV) &&
                string.IsNullOrWhiteSpace(variableSpecificCV) &&
                string.IsNullOrWhiteSpace(beneficialUse) &&
                string.IsNullOrWhiteSpace(reportingUnitUUID) &&
                string.IsNullOrWhiteSpace(geometry) &&
                string.IsNullOrWhiteSpace(reportingUnitTypeCV) &&
                string.IsNullOrWhiteSpace(usgsCategoryNameCV) &&
                string.IsNullOrWhiteSpace(state))
            {
                return new BadRequestObjectResult("At least one of the following filter parameters must be specified: variableCV, variableSpecificCV, beneficialUse, reportingUnitUUID, geometry, reportingUnitTypeCV, usgsCategoryNameCV, state");
            }

            var siteAllocationAmounts = await AggregatedAmountsManager.GetAggregatedAmountsAsync(new AggregatedAmountsFilters
            {
                BeneficialUse = beneficialUse,
                Geometry = geometry,
                ReportingUnitTypeCV = reportingUnitTypeCV,
                ReportingUnitUUID = reportingUnitUUID,
                UsgsCategoryNameCV = usgsCategoryNameCV,
                VariableCV = variableCV,
                VariableSpecificCV = variableSpecificCV,
                StartDate = startDate,
                EndDate = endDate,
                State = state
            }, startIndex, recordCount, geoFormat);
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

        private class AggregratedAmountsRequestBody
        {
            public string variableCV { get; set; }
            public string variableSpecificCV { get; set; }
            public string beneficialUseCV { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string reportingUnitUUID { get; set; }
            public string searchBoundary { get; set; }
            public string reportingUnitTypeCV { get; set; }
            public string usgsCategoryNameCV { get; set; }
            public string state { get; set; }
            public string startIndex { get; set; }
            public string recordCount { get; set; }
        }
    }
}
