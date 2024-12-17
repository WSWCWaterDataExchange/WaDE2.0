using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WaDEApiFunctions.v1
{
    public class WaterAllocation_SiteAllocationAmounts : FunctionBase
    {
        private readonly IWaterAllocationManager _waterAllocationManager;
        
        private readonly ILogger<WaterAllocation_SiteAllocationAmounts> _logger;
        
        public WaterAllocation_SiteAllocationAmounts(
            IWaterAllocationManager waterAllocationManager,
            ILogger<WaterAllocation_SiteAllocationAmounts> logger
        )
        {
            _waterAllocationManager = waterAllocationManager;
            _logger = logger;
        }

        
        [Function("WaterAllocation_SiteAllocationAmounts_v1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmounts")] HttpRequestData req)
        {
            _logger.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)} Run");

            var data = await Deserialize<SiteAllocationAmountsRequestBody>(req, _logger);
            
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
                return await CreateBadRequestResponse(
                    req,
                    new ValidationError("Filters",
                    [
                        "At least one of the following filter parameters must be specified: siteUuid, beneficialUseCv, geometry, siteTypeCV, usgsCategoryNameCV, huc8, huc12, county, state"
                    ])
                );
            }

            var siteAllocationAmounts = await _waterAllocationManager.GetSiteAllocationAmountsAsync(new SiteAllocationAmountsFilters
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

            return await CreateOkResponse(req, siteAllocationAmounts);
        }

        [Function("WaterAllocation_SiteAllocationAmountsDigest_v1")]
        public async Task<HttpResponseData> Digest([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/SiteAllocationAmountsDigest")] HttpRequestData req)
        {
            _logger.LogInformation($"Call to {nameof(WaterAllocation_SiteAllocationAmounts)} Digest");

            var data = await Deserialize<SiteAllocationAmountsDigestRequestBody>(req, _logger);

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

            if (string.IsNullOrWhiteSpace(organizationUUID) &&
                string.IsNullOrWhiteSpace(beneficialUseCv) &&
                string.IsNullOrWhiteSpace(geometry) &&
                string.IsNullOrWhiteSpace(siteTypeCV) &&
                string.IsNullOrWhiteSpace(usgsCategoryNameCV))
            {
                return await CreateBadRequestResponse(
                    req,
                    new ValidationError("Filters",
                    [
                        "At least one of the following filter parameters must be specified: organizationUUID, beneficialUseCv, geometry, siteTypeCV, usgsCategoryNameCV"
                    ])
                );
            }
            
            var siteAllocationAmounts = await _waterAllocationManager.GetSiteAllocationAmountsDigestAsync(new SiteAllocationAmountsDigestFilters
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

            return await CreateOkResponse(req, siteAllocationAmounts);
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
