using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace WaDEApiFunctions.v1
{
    public class WaterAllocation_SiteVariableAmounts : FunctionBase
    {
        private readonly ISiteVariableAmountsManager _siteVariableAmountsManager;
        
        private readonly ILogger<WaterAllocation_SiteVariableAmounts> _logger;

        public WaterAllocation_SiteVariableAmounts(
            ISiteVariableAmountsManager siteVariableAmountsManager,
            ILogger<WaterAllocation_SiteVariableAmounts> logger
        )
        {
            _siteVariableAmountsManager = siteVariableAmountsManager;
            _logger = logger;
        }

        [Function("WaterAllocation_SiteVariableAmounts_v1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteVariableAmounts")] HttpRequestData req)
        {
            _logger.LogInformation($"Call to {nameof(WaterAllocation_AggregatedAmounts)}");

            var data = await Deserialize<AggregratedAmountsRequestBody>(req, _logger);

            var siteUUID = req.GetQueryString("SiteUUID") ?? data?.siteUUID;
            var siteTypeCV = req.GetQueryString("SiteTypeCV") ?? data?.siteTypeCV;
            var variableCV = req.GetQueryString("VariableCV") ?? data?.variableCV;
            var variableSpecificCV = req.GetQueryString("VariableSpecificCV") ?? data?.variableCV;
            var beneficialUse = req.GetQueryString("BeneficialUseCV") ?? data?.beneficialUseCV;
            var usgsCategoryNameCV = req.GetQueryString("UsgsCategoryNameCV") ?? data?.usgsCategoryNameCV;
            var startDate = RequestDataParser.ParseDate(req.GetQueryString("StartDate") ?? data?.startDate);
            var endDate = RequestDataParser.ParseDate(req.GetQueryString("EndDate") ?? data?.endDate);
            var startDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("StartPublicationDate") ?? data?.startPublicationDate);
            var endDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("EndPublicationDate") ?? data?.endPublicationDate);
            var geometry = req.GetQueryString("SearchBoundary") ?? data?.searchBoundary;
            var huc8 = req.GetQueryString("HUC8") ?? data?.huc8;
            var huc12 = req.GetQueryString("HUC12") ?? data?.huc12;
            var county = req.GetQueryString("County") ?? data?.county;
            var state = req.GetQueryString("State") ?? data?.state;
            var startIndex = RequestDataParser.ParseInt(req.GetQueryString("StartIndex") ?? data?.startIndex) ?? 0;
            var recordCount = RequestDataParser.ParseInt(req.GetQueryString("RecordCount") ?? data?.recordCount) ?? 1000;
            var geoFormat = RequestDataParser.ParseGeometryFormat(req.GetQueryString("geoFormat")) ?? GeometryFormat.Wkt;

            if (string.IsNullOrWhiteSpace(variableCV) &&
                string.IsNullOrWhiteSpace(variableSpecificCV) &&
                string.IsNullOrWhiteSpace(beneficialUse) &&
                string.IsNullOrWhiteSpace(siteUUID) &&
                string.IsNullOrWhiteSpace(geometry) &&
                string.IsNullOrWhiteSpace(siteTypeCV) &&
                string.IsNullOrWhiteSpace(usgsCategoryNameCV) &&
                string.IsNullOrWhiteSpace(huc8) &&
                string.IsNullOrWhiteSpace(huc12) &&
                string.IsNullOrWhiteSpace(county) &&
                string.IsNullOrWhiteSpace(state))
            {
                return await CreateBadRequestResponse(
                    req,
                    new ValidationError(
                        "Filters",
                        [
                            "At least one of the following filter parameters must be specified: variableCV, variableSpecificCV, beneficialUse, siteUUID, geometry, siteTypeCV, usgsCategoryNameCV, huc8, huc12, county, state"
                        ]
                    ));
            }

            var siteAllocationAmounts = await _siteVariableAmountsManager.GetSiteVariableAmountsAsync(new SiteVariableAmountsFilters
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
                StartDataPublicationDate = startDataPublicationDate,
                EndDataPublicationDate = endDataPublicationDate,
                HUC8 = huc8,
                HUC12 = huc12,
                County = county,
                State = state
            }, startIndex, recordCount, geoFormat);
            
            return await CreateOkResponse(req, siteAllocationAmounts);
        }

        private sealed class AggregratedAmountsRequestBody
        {
            public string siteUUID { get; set; }
            public string siteTypeCV { get; set; }
            public string variableCV { get; set; }
            public string variableSpecificCV { get; set; }
            public string beneficialUseCV { get; set; }
            public string usgsCategoryNameCV { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string startPublicationDate { get; set; }
            public string endPublicationDate { get; set; }
            public string searchBoundary { get; set; }
            public string huc8 { get; set; }
            public string huc12 { get; set; }
            public string county { get; set; }
            public string state { get; set; }
            public string startIndex { get; set; }
            public string recordCount { get; set; }
        }
    }
}
