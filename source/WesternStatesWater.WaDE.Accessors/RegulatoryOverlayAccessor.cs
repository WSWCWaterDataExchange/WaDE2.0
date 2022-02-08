using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WaDE.Accessors.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Accessors
{
    public class RegulatoryOverlayAccessor : AccessorApi.IRegulatoryOverlayAccessor
    {
        public RegulatoryOverlayAccessor(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<WaterAllocationAccessor>();
        }

        private ILogger Logger { get; }
        private IConfiguration Configuration { get; set; }

        async Task<AccessorApi.RegulatoryReportingUnits> AccessorApi.IRegulatoryOverlayAccessor.GetRegulatoryReportingUnitsAsync(AccessorApi.RegulatoryOverlayFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var sw = Stopwatch.StartNew();
                var query = db.RegulatoryReportingUnitsFact.AsNoTracking();
                if (filters.StatutoryEffectiveDate != null)
                {
                    query = query.Where(a => a.RegulatoryOverlay.StatutoryEffectiveDate >= filters.StatutoryEffectiveDate);
                }
                if (filters.StatutoryEndDate != null)
                {
                    query = query.Where(a => a.RegulatoryOverlay.StatutoryEndDate <= filters.StatutoryEndDate);
                }
                if (!string.IsNullOrWhiteSpace(filters.OrganizationUUID))
                {
                    query = query.Where(a => a.Organization.OrganizationUuid == filters.OrganizationUUID);
                }
                if (!string.IsNullOrWhiteSpace(filters.RegulatoryOverlayUUID))
                {
                    query = query.Where(a => a.RegulatoryOverlay.RegulatoryOverlayUuid == filters.RegulatoryOverlayUUID);
                }
                if (!string.IsNullOrWhiteSpace(filters.RegulatoryStatusCV))
                {
                    query = query.Where(a => a.RegulatoryOverlay.RegulatoryStatusCv == filters.RegulatoryStatusCV);
                }
                if (!string.IsNullOrWhiteSpace(filters.ReportingUnitUUID))
                {
                    query = query.Where(a => a.ReportingUnit.ReportingUnitUuid == filters.ReportingUnitUUID);
                }
                if (!string.IsNullOrWhiteSpace(filters.Geometry))
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    WKTReader reader = new WKTReader(geometryFactory);
                    var shape = reader.Read(filters.Geometry);
                    query = query.Where(a => a.ReportingUnit.Geometry != null && a.ReportingUnit.Geometry.Intersects(shape));
                }
                if (!string.IsNullOrWhiteSpace(filters.State))
                {
                    query = query.Where(a => a.Organization.State == filters.State);
                }

                var totalCount = query.Count();

                var results = await query
                    .OrderBy(a => a.BridgeId)
                    .Skip(startIndex)
                    .Take(recordCount)
                    .ProjectTo<ReportingUnitRegulatoryHelper>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var orgIds = results.Select(a => a.OrganizationId).ToHashSet();
                var orgsTask = db.OrganizationsDim
                    .Where(a => orgIds.Contains(a.OrganizationId))
                    .ProjectTo<AccessorApi.RegulatoryReportingUnitsOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var regulatoryOverlayIds = results.Select(a => a.RegulatoryOverlayId).ToList();
                var regulatoryOverlaysTask = db.RegulatoryOverlayDim
                    .Where(a => regulatoryOverlayIds.Contains(a.RegulatoryOverlayId))
                    .ProjectTo<AccessorApi.RegulatoryOverlay>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var regulatoryOverlays = await regulatoryOverlaysTask;

                var regulatoryReportingUnitsOrganizations = new List<AccessorApi.RegulatoryReportingUnitsOrganization>();
                foreach (var org in await orgsTask)
                {
                    ProcessRegulatoryReportingUnitsOrganization(org, results, regulatoryOverlays);
                    regulatoryReportingUnitsOrganizations.Add(org);
                }

                sw.Stop();
                Logger.LogInformation($"Completed RegulatoryOverlay [{sw.ElapsedMilliseconds} ms]");
                return new AccessorApi.RegulatoryReportingUnits
                {
                    TotalRegulatoryReportingUnitsCount = totalCount,
                    Organizations = regulatoryReportingUnitsOrganizations
                };
            }
        }

        private static void ProcessRegulatoryReportingUnitsOrganization(
            AccessorApi.RegulatoryReportingUnitsOrganization org,
            List<ReportingUnitRegulatoryHelper> results,
            List<AccessorApi.RegulatoryOverlay> regulatoryOverlays)
        {
            var regulatoryReportingUnits = results.Where(a => a.OrganizationId == org.OrganizationId).ToList();
            var regulatoryOverlayIds2 = regulatoryReportingUnits.Select(a => a.RegulatoryOverlayId).ToList();

            org.RegulatoryOverlays = regulatoryOverlays
                .Where(a => regulatoryOverlayIds2.Contains(a.RegulatoryOverlayID))
                .Map<List<AccessorApi.RegulatoryOverlay>>();

            org.ReportingUnitsRegulatory = regulatoryReportingUnits.Map<List<AccessorApi.ReportingUnitRegulatory>>();
        }

        internal class ReportingUnitRegulatoryHelper
        {
            public long OrganizationId { get; set; }
            public long RegulatoryOverlayId { get; set; }
            public string ReportingUnitUUID { get; set; }
            public string ReportingUnitNativeID { get; set; }
            public string ReportingUnitName { get; set; }
            public string ReportingUnitTypeCV { get; set; }
            public string ReportingUnitUpdateDate { get; set; }
            public string ReportingUnitProductVersion { get; set; }
            public string StateCV { get; set; }
            public string EPSGCodeCV { get; set; }
            public string Geometry { get; set; }
        }
    }
}
