using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Accessors
{
    public class AggregratedAmountsAccessor : AccessorApi.IAggregatedAmountsAccessor
    {
        public AggregratedAmountsAccessor(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<AggregratedAmountsAccessor>();
        }

        private ILogger Logger { get; }
        private IConfiguration Configuration { get; set; }

        async Task<AccessorApi.AggregatedAmounts> AccessorApi.IAggregatedAmountsAccessor.GetAggregatedAmountsAsync(AccessorApi.AggregatedAmountsFilters filters, int startIndex, int recordCount)
        {
            var sw = Stopwatch.StartNew();

            var totalCountTask = GetAggregatedAmountsCount(filters).BlockTaskInTransaction();
            var results = await GetAggregatedAmounts(filters, startIndex, recordCount).BlockTaskInTransaction();

            var reportingUnitIds = results.Select(a => a.ReportingUnitId).ToHashSet();

            var beneficialUseTask = GetBeneficialUses(results.Select(a => a.AggregatedAmountId).ToHashSet()).BlockTaskInTransaction();
            var orgsTask = GetOrganizations(results.Select(a => a.OrganizationId).ToHashSet()).BlockTaskInTransaction();
            var waterSourceTask = GetWaterSources(results.Select(a => a.WaterSourceId).ToHashSet()).BlockTaskInTransaction();
            var reportingUnitsTask = GetReportingUnits(reportingUnitIds).BlockTaskInTransaction();
            var methodTask = GetMethods(results.Select(a => a.MethodId).ToHashSet()).BlockTaskInTransaction();
            var variableSpecificTask = GetVariables(results.Select(a => a.VariableSpecificId).ToHashSet()).BlockTaskInTransaction();
            var regulatoryOverlayTask = GetRegulatoryReportingUnits(reportingUnitIds).BlockTaskInTransaction();

            var beneficialUses = await beneficialUseTask;
            var waterSources = await waterSourceTask;
            var variableSpecifics = await variableSpecificTask;
            var methods = await methodTask;
            var reportingUnits = await reportingUnitsTask;
            var regulatoryOverlays = await regulatoryOverlayTask;

            var waterAllocationOrganizations = new List<AccessorApi.AggregatedAmountsOrganization>();
            foreach (var org in await orgsTask)
            {
                ProcessAggregatedAmountsOrganization(org, results, waterSources, variableSpecifics, reportingUnits, methods, beneficialUses, regulatoryOverlays);
                waterAllocationOrganizations.Add(org);
            }

            sw.Stop();
            Logger.LogInformation($"Completed AggregatedAmounts [{sw.ElapsedMilliseconds} ms]");
            return new AccessorApi.AggregatedAmounts
            {
                TotalAggregatedAmountsCount = await totalCountTask,
                Organizations = waterAllocationOrganizations
            };
        }

        private static IQueryable<AggregatedAmountsFact> BuildQuery(AccessorApi.AggregatedAmountsFilters filters, WaDEContext db)
        {
            var query = db.AggregatedAmountsFact.AsNoTracking();

            if (filters.StartDate != null)
            {
                query = query.Where(a => a.TimeframeStart.Date >= filters.StartDate);
            }
            if (filters.EndDate != null)
            {
                query = query.Where(a => a.TimeframeEnd.Date <= filters.EndDate);
            }
            if (!string.IsNullOrWhiteSpace(filters.VariableCV))
            {
                query = query.Where(a => a.VariableSpecific.VariableCv == filters.VariableCV);
            }
            if (!string.IsNullOrWhiteSpace(filters.VariableSpecificCV))
            {
                query = query.Where(a => a.VariableSpecific.VariableSpecificCv == filters.VariableSpecificCV);
            }
            if (!string.IsNullOrWhiteSpace(filters.BeneficialUse))
            {
                query = query.Where(a => a.PrimaryBeneficialUse.Name == filters.BeneficialUse || a.AggBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.Name == filters.BeneficialUse));
            }
            if (!string.IsNullOrWhiteSpace(filters.ReportingUnitUUID))
            {
                query = query.Where(a => a.ReportingUnit.ReportingUnitUuid == filters.ReportingUnitUUID);
            }
            if (!string.IsNullOrWhiteSpace(filters.ReportingUnitTypeCV))
            {
                query = query.Where(a => a.ReportingUnit.ReportingUnitTypeCv == filters.ReportingUnitTypeCV);
            }
            if (!string.IsNullOrWhiteSpace(filters.UsgsCategoryNameCV))
            {
                query = query.Where(a => a.PrimaryBeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCV || a.AggBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCV));
            }
            if (!string.IsNullOrWhiteSpace(filters.Geometry))
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var reader = new WKTReader(geometryFactory.GeometryServices);
                var shape = reader.Read(filters.Geometry);
                query = query.Where(a => a.ReportingUnit.Geometry != null && a.ReportingUnit.Geometry.Intersects(shape));
            }
            if (!string.IsNullOrWhiteSpace(filters.State))
            {
                query = query.Where(a => a.Organization.State == filters.State);
            }

            return query;
        }

        private async Task<List<AggregatedHelper>> GetAggregatedAmounts(AccessorApi.AggregatedAmountsFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildQuery(filters, db)
                                .OrderBy(a => a.AggregatedAmountId)
                                .Skip(startIndex)
                                .Take(recordCount)
                                .ProjectTo<AggregatedHelper>(Mapping.DtoMapper.Configuration)
                                .ToListAsync();
            }
        }

        private async Task<int> GetAggregatedAmountsCount(AccessorApi.AggregatedAmountsFilters filters)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildQuery(filters, db).CountAsync();
            }
        }

        private async ValueTask<List<(long AggregatedAmountId, BeneficialUsesCV BeneficialUse)>> GetBeneficialUses(HashSet<long> aggregatedIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.AggBridgeBeneficialUsesFact
                               .AsNoTracking()
                               .Where(a => aggregatedIds.Contains(a.AggregatedAmountId))
                               .Select(a => new { a.AggregatedAmountId, a.BeneficialUse })
                               .AsAsyncEnumerable()
                               .Select(a => (a.AggregatedAmountId, a.BeneficialUse))
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.AggregatedAmountsOrganization>> GetOrganizations(HashSet<long> orgIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.OrganizationsDim
                               .AsNoTracking()
                               .Where(a => orgIds.Contains(a.OrganizationId))
                               .ProjectTo<AccessorApi.AggregatedAmountsOrganization>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.WaterSource>> GetWaterSources(HashSet<long> waterSourceIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.WaterSourcesDim
                               .AsNoTracking()
                               .Where(a => waterSourceIds.Contains(a.WaterSourceId))
                               .ProjectTo<AccessorApi.WaterSource>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.ReportingUnit>> GetReportingUnits(HashSet<long> reportingUnitIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.ReportingUnitsDim
                               .AsNoTracking()
                               .Where(a => reportingUnitIds.Contains(a.ReportingUnitId))
                               .ProjectTo<AccessorApi.ReportingUnit>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.Method>> GetMethods(HashSet<long> methodIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.MethodsDim
                               .AsNoTracking()
                               .Where(a => methodIds.Contains(a.MethodId))
                               .ProjectTo<AccessorApi.Method>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.VariableSpecific>> GetVariables(HashSet<long> variableSpecificIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.VariablesDim
                               .AsNoTracking()
                               .Where(a => variableSpecificIds.Contains(a.VariableSpecificId))
                               .ProjectTo<AccessorApi.VariableSpecific>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async ValueTask<List<(long ReportingUnitId, long RegulatoryOverlayId, RegulatoryOverlayDim RegulatoryOverlay)>> GetRegulatoryReportingUnits(HashSet<long> reportingUnitIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.RegulatoryReportingUnitsFact
                               .AsNoTracking()
                               .Where(a => reportingUnitIds.Contains(a.ReportingUnitId))
                               .Select(a => new { a.ReportingUnitId, a.RegulatoryOverlayId, a.RegulatoryOverlay })
                               .AsAsyncEnumerable()
                               .Select(a => (a.ReportingUnitId, a.RegulatoryOverlayId, a.RegulatoryOverlay))
                               .ToListAsync();
            }
        }

        private static void ProcessAggregatedAmountsOrganization(AccessorApi.AggregatedAmountsOrganization org, List<AggregatedHelper> results,
            List<AccessorApi.WaterSource> waterSources, List<AccessorApi.VariableSpecific> variableSpecifics, List<AccessorApi.ReportingUnit> reportingUnits, List<AccessorApi.Method> methods, List<(long AggregatedAmountId, BeneficialUsesCV BeneficialUse)> beneficialUses, List<(long ReportingUnitId, long RegulatoryOverlayId, RegulatoryOverlayDim RegulatoryOverlay)> regulatoryOverlays)
        {
            var allocations = results.Where(a => a.OrganizationId == org.OrganizationId).ToList();

            var allocationIds = allocations.Select(a => a.AggregatedAmountId).ToHashSet();
            var waterSourceIds = allocations.Select(a => a.WaterSourceId).ToHashSet();
            var variableSpecificIds = allocations.Select(a => a.VariableSpecificId).ToHashSet();
            var methodIds = allocations.Select(a => a.MethodId).ToHashSet();
            var reportingUnitIds = allocations.Select(a => a.ReportingUnitId).ToHashSet();
            var regulatoryOverlayIds = regulatoryOverlays.Select(a => a.RegulatoryOverlayId).ToHashSet();

            org.WaterSources = waterSources
                .Where(a => waterSourceIds.Contains(a.WaterSourceId))
                .Map<List<AccessorApi.WaterSource>>();

            org.VariableSpecifics = variableSpecifics
                .Where(a => variableSpecificIds.Contains(a.VariableSpecificId))
                .Map<List<AccessorApi.VariableSpecific>>();

            org.ReportingUnits = reportingUnits
                .Where(a => reportingUnitIds.Contains(a.ReportingUnitId))
                .Map<List<AccessorApi.ReportingUnit>>();

            org.Methods = methods
                .Where(a => methodIds.Contains(a.MethodId))
                .Map<List<AccessorApi.Method>>();

            org.BeneficialUses = beneficialUses
                .Where(a => allocationIds.Contains(a.AggregatedAmountId))
                .Select(a => a.BeneficialUse)
                .DistinctBy(a => a.Name)
                .Map<List<AccessorApi.BeneficialUse>>();

            org.RegulatoryOverlays = regulatoryOverlays
                .Where(a => regulatoryOverlayIds.Contains(a.RegulatoryOverlayId))
                .Select(a => a.RegulatoryOverlay)
                .DistinctBy(a => a.RegulatoryOverlayUuid)
                .Map<List<AccessorApi.RegulatoryOverlay>>();

            org.AggregatedAmounts = allocations.Map<List<AccessorApi.AggregatedAmount>>();

            foreach (var waterAllocation in org.AggregatedAmounts)
            {
                waterAllocation.BeneficialUses = beneficialUses
                    .Where(a => a.AggregatedAmountId == waterAllocation.AggregatedAmountId)
                    .Select(a => a.BeneficialUse.Name).ToList();
            }

            foreach (var reportingUnit in org.ReportingUnits)
            {
                reportingUnit.RegulatoryOverlayUUIDs = regulatoryOverlays
                    .Where(a => a.ReportingUnitId == reportingUnit.ReportingUnitId)
                    .DistinctBy(a => a.RegulatoryOverlayId)
                    .Select(a => a.RegulatoryOverlay.RegulatoryOverlayUuid).ToList();
            }
        }

        internal class AggregatedHelper
        {
            public long AggregatedAmountId { get; set; }
            public string Variable { get; set; }
            public string VariableSpecificTypeCV { get; set; }
            public string MethodUUID { get; set; }
            public string ReportYear { get; set; }
            public DateTime? TimeframeStart { get; set; }
            public DateTime? TimeframeEnd { get; set; }
            public string WaterSourceUUID { get; set; }
            public string ReportingUnitUUID { get; set; }
            public double Amount { get; set; }
            public long? PopulationServed { get; set; }
            public double? PowerGeneratedGWh { get; set; }
            public double? IrrigatedAcreage { get; set; }
            public DateTime? DataPublicationDate { get; set; }
            public string PrimaryUse { get; set; }

            public long OrganizationId { get; set; }
            public long WaterSourceId { get; set; }
            public long ReportingUnitId { get; set; }
            public long MethodId { get; set; }
            public long VariableSpecificId { get; set; }
        }
    }
}
