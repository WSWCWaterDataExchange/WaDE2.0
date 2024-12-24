using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.Shared.Errors;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V1;
using WesternStatesWater.WaDE.Contracts.Api.Responses.V2;

namespace WaDEApiFunctions.v1
{
    public class WaterAllocation_RegulatoryOverlay : FunctionBase
    {
        private readonly IWaterResourceManager _waterResourceManager;
        
        private readonly ILogger<WaterAllocation_RegulatoryOverlay> _logger;
        
        public WaterAllocation_RegulatoryOverlay(
            IWaterResourceManager waterResourceManager,
            ILogger<WaterAllocation_RegulatoryOverlay> logger
        )
        {
            _waterResourceManager = waterResourceManager;
            _logger = logger;
        }


        [Function("WaterAllocation_RegulatoryOverlay_v1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/AggRegulatoryOverlay")] HttpRequestData req)
        {
            _logger.LogInformation($"Call to {nameof(WaterAllocation_RegulatoryOverlay)}");

            var data = await Deserialize<RegulatoryOverlayRequestBody>(req, _logger);            

            var reportingUnitUUID = ((string)req.Query["ReportingUnitUUID"]) ?? data?.reportingUnitUUID;
            var regulatoryOverlayUUID = ((string)req.Query["RegulatoryOverlayUUID"]) ?? data?.regulatoryOverlayUUID;
            var organizationUUID = ((string)req.Query["OrganizationUUID"]) ?? data?.organizationUUID;
            var statutoryEffectiveDate = RequestDataParser.ParseDate(((string)req.Query["StatutoryEffectiveDate"]) ?? data?.statutoryEffectiveDate);
            var statutoryEndDate = RequestDataParser.ParseDate(((string)req.Query["StatutoryEndDate"]) ?? data?.statutoryEndDate);
            var startDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("StartPublicationDate") ?? data?.startPublicationDate);
            var endDataPublicationDate = RequestDataParser.ParseDate(req.GetQueryString("EndPublicationDate") ?? data?.endPublicationDate);
            var regulatoryStatusCV = ((string)req.Query["RegulatoryStatusCV"]) ?? data?.regulatoryStatusCV;
            var geometry = ((string)req.Query["SearchBoundary"]) ?? data?.searchBoundary;
            var state = ((string)req.Query["State"]) ?? data?.state;
            var startIndex = RequestDataParser.ParseInt(((string)req.Query["StartIndex"]) ?? data?.startIndex) ?? 0;
            var recordCount = RequestDataParser.ParseInt(((string)req.Query["RecordCount"]) ?? data?.recordCount) ?? 1000;
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

            if (string.IsNullOrWhiteSpace(reportingUnitUUID) &&
                string.IsNullOrWhiteSpace(regulatoryOverlayUUID) &&
                string.IsNullOrWhiteSpace(organizationUUID) &&
                string.IsNullOrWhiteSpace(regulatoryStatusCV) &&
                string.IsNullOrWhiteSpace(geometry) &&
                string.IsNullOrWhiteSpace(state))
            {
                return await CreateBadRequestResponse(
                    req,
                    new ValidationError("Filters", ["At least one of the following filter parameters must be specified: reportingUnitUUID, regulatoryOverlayUUID, organizationUUID, regulatoryStatusCV, geometry, state"])
                );
            }

            var request = new OverlayResourceSearchRequest
            {
                Filters = new RegulatoryOverlayFilters
                {
                    ReportingUnitUUID = reportingUnitUUID,
                    RegulatoryOverlayUUID = regulatoryOverlayUUID,
                    OrganizationUUID = organizationUUID,
                    StatutoryEffectiveDate = statutoryEffectiveDate,
                    StatutoryEndDate = statutoryEndDate,
                    StartDataPublicationDate = startDataPublicationDate,
                    EndDataPublicationDate = endDataPublicationDate,
                    RegulatoryStatusCV = regulatoryStatusCV,
                    Geometry = geometry,
                    State = state
                },
                StartIndex = startIndex,
                RecordCount = recordCount,
                OutputGeometryFormat = geoFormat
            };

            var regulatoryReportingUnits = await _waterResourceManager
                .Load<OverlayResourceSearchRequest, OverlayResourceSearchResponse>(request);
            
            return await CreateOkResponse(req, regulatoryReportingUnits);
        }

        private sealed class RegulatoryOverlayRequestBody
        {
            public string reportingUnitUUID { get; set; }
            public string regulatoryOverlayUUID { get; set; }
            public string organizationUUID { get; set; }
            public string statutoryEffectiveDate { get; set; }
            public string statutoryEndDate { get; set; }
            public string startPublicationDate { get; set; }
            public string endPublicationDate { get; set; }
            public string regulatoryStatusCV { get; set; }
            public string searchBoundary { get; set; }
            public string state { get; set; }
            public string startIndex { get; set; }
            public string recordCount { get; set; }
        }
    }
}
