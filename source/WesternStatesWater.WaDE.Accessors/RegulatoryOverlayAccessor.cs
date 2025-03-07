using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Requests;
using WesternStatesWater.WaDE.Accessors.Contracts.Api.V2.Responses;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Handlers;
using WesternStatesWater.WaDE.Accessors.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Accessors
{
    public class RegulatoryOverlayAccessor : AccessorBase, AccessorApi.IRegulatoryOverlayAccessor
    {
        public RegulatoryOverlayAccessor(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IAccessorRequestHandlerResolver requestHandlerResolver
        ) : base(requestHandlerResolver)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<WaterAllocationAccessor>();
        }

        private ILogger Logger { get; }
        private IConfiguration Configuration { get; set; }

        async Task<AccessorApi.RegulatoryReportingUnits> AccessorApi.IRegulatoryOverlayAccessor.
            GetRegulatoryReportingUnitsAsync(AccessorApi.RegulatoryOverlayFilters filters, int startIndex,
                int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var sw = Stopwatch.StartNew();

                var totalCountTask = GetRegulatoryReportingUnitsCount(filters).BlockTaskInTransaction();
                var results = await GetRegulatoryReportingUnits(filters, startIndex, recordCount)
                    .BlockTaskInTransaction();

                var orgsTask = GetOrganizations(results.Select(a => a.OrganizationId).ToHashSet())
                    .BlockTaskInTransaction();
                var regulatoryOverlaysTask =
                    GetRegulatoryOverlays(results.Select(a => a.RegulatoryOverlayId).ToHashSet())
                        .BlockTaskInTransaction();

                var regulatoryOverlays = await regulatoryOverlaysTask;

                var regulatoryReportingUnitsOrganizations =
                    new List<AccessorApi.RegulatoryReportingUnitsOrganization>();
                foreach (var org in await orgsTask)
                {
                    ProcessRegulatoryReportingUnitsOrganization(org, results, regulatoryOverlays);
                    regulatoryReportingUnitsOrganizations.Add(org);
                }

                sw.Stop();
                Logger.LogInformation($"Completed RegulatoryOverlay [{sw.ElapsedMilliseconds} ms]");
                return new AccessorApi.RegulatoryReportingUnits
                {
                    TotalRegulatoryReportingUnitsCount = await totalCountTask,
                    Organizations = regulatoryReportingUnitsOrganizations
                };
            }
        }

        public async Task<AccessorApi.OverlayMetadata> GetOverlayMetadata()
        {
            return await Task.FromResult(new AccessorApi.OverlayMetadata
            {
                BoundaryBox = new AccessorApi.BoundaryBox
                {
                    Crs = "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
                    MinX = -180,
                    MinY = 18,
                    MaxX = -93,
                    MaxY = 72
                }
            });
        }

        public async Task<TResponse> Search<TRequest, TResponse>(TRequest request) where TRequest : SearchRequestBase
            where TResponse : SearchResponseBase
        {
            return await ExecuteAsync<TRequest, TResponse>(request);
        }

        private static IQueryable<RegulatoryReportingUnitsFact> BuildRegulatoryReportingUnitsQuery(
            AccessorApi.RegulatoryOverlayFilters filters, WaDEContext db)
        {
            var query = db.RegulatoryReportingUnitsFact.AsNoTracking();
            if (filters.StatutoryEffectiveDate != null)
            {
                query = query.Where(a => a.RegulatoryOverlay.StatutoryEffectiveDate >= filters.StatutoryEffectiveDate);
            }

            if (filters.StatutoryEndDate != null)
            {
                query = query.Where(a => a.RegulatoryOverlay.StatutoryEndDate <= filters.StatutoryEndDate);
            }

            if (filters.StartDataPublicationDate != null)
            {
                query = query.Where(a => a.DataPublicationDate.Date >= filters.StartDataPublicationDate);
            }

            if (filters.EndDataPublicationDate != null)
            {
                query = query.Where(a => a.DataPublicationDate.Date <= filters.EndDataPublicationDate);
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

            if (filters.Geometry != null)
            {
                query = query.Where(a =>
                    a.ReportingUnit.Geometry != null && a.ReportingUnit.Geometry.Intersects(filters.Geometry));
            }

            if (!string.IsNullOrWhiteSpace(filters.State))
            {
                query = query.Where(a => a.Organization.State == filters.State);
            }

            return query;
        }

        private async Task<int> GetRegulatoryReportingUnitsCount(AccessorApi.RegulatoryOverlayFilters filters)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildRegulatoryReportingUnitsQuery(filters, db).CountAsync();
            }
        }

        private async Task<List<ReportingUnitRegulatoryHelper>> GetRegulatoryReportingUnits(
            AccessorApi.RegulatoryOverlayFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildRegulatoryReportingUnitsQuery(filters, db)
                    .OrderBy(a => a.BridgeId)
                    .Skip(startIndex)
                    .Take(recordCount)
                    .ProjectTo<ReportingUnitRegulatoryHelper>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.RegulatoryReportingUnitsOrganization>> GetOrganizations(
            HashSet<long> orgIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.OrganizationsDim
                    .Where(a => orgIds.Contains(a.OrganizationId))
                    .ProjectTo<AccessorApi.RegulatoryReportingUnitsOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.RegulatoryOverlay>> GetRegulatoryOverlays(
            HashSet<long> regulatoryOverlayIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.RegulatoryOverlayDim
                    .Where(a => regulatoryOverlayIds.Contains(a.RegulatoryOverlayId))
                    .ProjectTo<AccessorApi.RegulatoryOverlay>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();
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
            public Geometry Geometry { get; set; }
        }
    }
}