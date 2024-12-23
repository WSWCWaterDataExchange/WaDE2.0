using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.Shared.Errors;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

namespace WaDEApiFunctions.v1
{
    public class WaterAllocation_AggregatedAmounts : FunctionBase
    {
        private readonly IAggregatedAmountsManager _aggregatedAmountsManager;
        
        private readonly ILogger<WaterAllocation_AggregatedAmounts> _logger;

        public WaterAllocation_AggregatedAmounts(
            IAggregatedAmountsManager aggregatedAmountsManager,
            ILogger<WaterAllocation_AggregatedAmounts> logger
        )
        {
            _aggregatedAmountsManager = aggregatedAmountsManager;
            _logger = logger;
        }

        [Function("WaterAllocation_AggregatedAmounts_v1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/AggregatedAmounts")] HttpRequestData req)
        {
            _logger.LogInformation($"Call to {nameof(WaterAllocation_AggregatedAmounts)}");

            var data = await Deserialize<AggregratedAmountsRequestBody>(req, _logger);

            var variableCV = req.GetQueryString("VariableCV") ?? data?.variableCV;
            var variableSpecificCV = req.GetQueryString("VariableSpecificCV") ?? data?.variableCV;
            var beneficialUse = req.GetQueryString("BeneficialUseCV") ?? data?.beneficialUseCV;
            var startDate = RequestDataParser.ParseDate(req.GetQueryString("StartDate") ?? data?.startDate);
            var endDate = RequestDataParser.ParseDate(req.GetQueryString("EndDate") ?? data?.endDate);
            var startDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("StartPublicationDate") ?? data?.startPublicationDate);
            var endDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("EndPublicationDate") ?? data?.endPublicationDate);
            var reportingUnitUUID = req.GetQueryString("ReportingUnitUUID") ?? data?.reportingUnitUUID;
            var geometry = req.GetQueryString("SearchBoundary") ?? data?.searchBoundary;
            var reportingUnitTypeCV = req.GetQueryString("ReportingUnitTypeCV") ?? data?.reportingUnitTypeCV;
            var usgsCategoryNameCV = req.GetQueryString("UsgsCategoryNameCV") ?? data?.usgsCategoryNameCV;
            var state = req.GetQueryString("State") ?? data?.state;
            var startIndex = RequestDataParser.ParseInt(req.GetQueryString("StartIndex") ?? data?.startIndex) ?? 0;
            var recordCount = RequestDataParser.ParseInt(req.GetQueryString("RecordCount") ?? data?.recordCount) ?? 1000;
            var geoFormat = RequestDataParser.ParseGeometryFormat(req.GetQueryString("geoFormat")) ?? GeometryFormat.Wkt;

            if (startIndex < 0)
            {
                return await CreateBadRequestResponse(
                    req,
                    new ValidationError("StartIndex", ["StartIndex must be 0 or greater."])
                );
            }

            if (recordCount is < 1 or > 10000)
            {
                return await CreateBadRequestResponse(
                    req,
                    new ValidationError("RecordCount", ["RecordCount must be between 1 and 10000"])
                );
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
                return await CreateBadRequestResponse(
                    req,
                    new ValidationError(
                        "Filters",
                        [
                            "At least one of the following filter parameters must be specified: variableCV, variableSpecificCV, beneficialUse, reportingUnitUUID, geometry, reportingUnitTypeCV, usgsCategoryNameCV, state"
                        ]
                    ));
            }

            var searchRequest = new AggregatedAmountsSearchRequest
            {
                Filters = new AggregatedAmountsFilters
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
                    StartDataPublicationDate = startDataPublicationDate,
                    EndDataPublicationDate = endDataPublicationDate,
                    State = state
                },
                StartIndex = startIndex,
                RecordCount = recordCount,
                OutputGeometryFormat = geoFormat
            };

            var siteAllocationAmounts = await _aggregatedAmountsManager.Load(searchRequest);
            
            return await CreateOkResponse(req, siteAllocationAmounts);
        }

        private sealed class AggregratedAmountsRequestBody
        {
            public string variableCV { get; set; }
            public string variableSpecificCV { get; set; }
            public string beneficialUseCV { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string startPublicationDate { get; set; }
            public string endPublicationDate { get; set; }
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
