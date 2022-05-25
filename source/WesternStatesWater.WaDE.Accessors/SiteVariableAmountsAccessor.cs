using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
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
    public class SiteVariableAmountsAccessor : AccessorApi.ISiteVariableAmountsAccessor
    {
        public SiteVariableAmountsAccessor(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<AggregratedAmountsAccessor>();
        }

        private ILogger Logger { get; }
        private IConfiguration Configuration { get; set; }

        async Task<AccessorApi.SiteVariableAmounts> AccessorApi.ISiteVariableAmountsAccessor.GetSiteVariableAmountsAsync(AccessorApi.SiteVariableAmountsFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var sw = Stopwatch.StartNew();

                var totalCountTask = GetSiteVariableAmountsCount(filters).BlockTaskInTransaction();
                var results = await GetSiteVariableAmounts(filters, startIndex, recordCount).BlockTaskInTransaction();

                var orgsTask = GetSiteVariableAmountsOrganizations(results.Select(a => a.OrganizationId).ToHashSet()).BlockTaskInTransaction();
                var waterSourceTask = GetWaterSources(results.Select(a => a.WaterSourceId).ToHashSet()).BlockTaskInTransaction();
                var variableSpecificTask = GetVariables(results.Select(a => a.VariableSpecificId).ToHashSet()).BlockTaskInTransaction();
                var methodTask = GetMethods(results.Select(a => a.MethodId).ToHashSet()).BlockTaskInTransaction();
                var beneficialUseTask = GetBeneficialUses(results.Select(a => a.SiteVariableAmountId).ToHashSet()).BlockTaskInTransaction();
                HashSet<long> siteIds = results.Select(a => a.SiteID).ToHashSet();
                var siteTask = GetSites(siteIds).BlockTaskInTransaction();
                var siteRelationshipsTask = GetPodPouSites(siteIds).BlockTaskInTransaction();

                var beneficialUses = await beneficialUseTask;
                var waterSources = await waterSourceTask;
                var variableSpecifics = await variableSpecificTask;
                var methods = await methodTask;
                var sites = await siteTask;
                var siteRelationships = await siteRelationshipsTask;

                var siteVariableAmountsOrganizations = new List<AccessorApi.SiteVariableAmountsOrganization>();
                foreach (var org in await orgsTask)
                {
                    ProcessSiteVariableAmountsOrganization(org, results, waterSources, variableSpecifics, methods, beneficialUses, sites, siteRelationships);
                    siteVariableAmountsOrganizations.Add(org);
                }

                sw.Stop();
                Logger.LogInformation($"Completed SiteVariableAmounts [{sw.ElapsedMilliseconds } ms]");
                return new AccessorApi.SiteVariableAmounts
                {
                    TotalSiteVariableAmountsCount = await totalCountTask,
                    Organizations = siteVariableAmountsOrganizations
                };
            }
        }

        private static IQueryable<SiteVariableAmountsFact> BuildQuery(AccessorApi.SiteVariableAmountsFilters filters, WaDEContext db)
        {
            var query = db.SiteVariableAmountsFact.AsNoTracking();
            if (filters.TimeframeStartDate != null)
            {
                query = query.Where(a => a.TimeframeStartNavigation.Date >= filters.TimeframeStartDate);
            }
            if (filters.TimeframeEndDate != null)
            {
                query = query.Where(a => a.TimeframeEndNavigation.Date <= filters.TimeframeEndDate);
            }
            if (filters.StartDataPublicationDate != null)
            {
                query = query.Where(a => a.DataPublicationDateNavigation.Date >= filters.StartDataPublicationDate);
            }
            if (filters.EndDataPublicationDate != null)
            {
                query = query.Where(a => a.DataPublicationDateNavigation.Date <= filters.EndDataPublicationDate);
            }
            if (!string.IsNullOrWhiteSpace(filters.VariableCv))
            {
                query = query.Where(a => a.VariableSpecific.VariableCv == filters.VariableCv);
            }
            if (!string.IsNullOrWhiteSpace(filters.VariableSpecificCv))
            {
                query = query.Where(a => a.VariableSpecific.VariableSpecificCv == filters.VariableSpecificCv);
            }
            if (!string.IsNullOrWhiteSpace(filters.BeneficialUseCv))
            {
                query = query.Where(a => a.PrimaryUseCategoryCV == filters.BeneficialUseCv || a.SitesBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.Name == filters.BeneficialUseCv));
            }
            if (!string.IsNullOrWhiteSpace(filters.UsgsCategoryNameCv))
            {
                query = query.Where(a => a.PrimaryBeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCv || a.SitesBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCv));
            }
            if (!string.IsNullOrWhiteSpace(filters.SiteUuid))
            {
                query = query.Where(a => a.Site.SiteUuid == filters.SiteUuid);
            }
            if (!string.IsNullOrWhiteSpace(filters.SiteTypeCv))
            {
                query = query.Where(a => a.Site.SiteTypeCv == filters.SiteTypeCv);
            }
            if (filters.Geometry != null)
            {
                query = query.Where(a => (a.Site.Geometry != null && a.Site.Geometry.Intersects(filters.Geometry)) ||
                                         (a.Site.SitePoint != null && a.Site.SitePoint.Intersects(filters.Geometry)));
            }
            if (!string.IsNullOrWhiteSpace(filters.HUC8))
            {
                query = query.Where(a => a.Site.HUC8 == filters.HUC8);
            }
            if (!string.IsNullOrWhiteSpace(filters.HUC12))
            {
                query = query.Where(a => a.Site.HUC12 == filters.HUC12);
            }
            if (!string.IsNullOrWhiteSpace(filters.County))
            {
                query = query.Where(a => a.Site.County == filters.County);
            }
            if (!string.IsNullOrWhiteSpace(filters.State))
            {
                query = query.Where(a => a.Organization.State == filters.State);
            }
            return query;
        }

        private async Task<List<SiteVariableAmountHelper>> GetSiteVariableAmounts(AccessorApi.SiteVariableAmountsFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildQuery(filters, db)
                                .OrderBy(a => a.SiteVariableAmountId)
                                .Skip(startIndex)
                                .Take(recordCount)
                                .ProjectTo<SiteVariableAmountHelper>(Mapping.DtoMapper.Configuration)
                                .ToListAsync();
            }
        }

        private async Task<int> GetSiteVariableAmountsCount(AccessorApi.SiteVariableAmountsFilters filters)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildQuery(filters, db).CountAsync();
            }
        }

        private async Task<List<AccessorApi.SiteVariableAmountsOrganization>> GetSiteVariableAmountsOrganizations(HashSet<long> orgIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.OrganizationsDim
                               .Where(a => orgIds.Contains(a.OrganizationId))
                               .ProjectTo<AccessorApi.SiteVariableAmountsOrganization>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.WaterSource>> GetWaterSources(HashSet<long> waterSourceIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.WaterSourcesDim
                               .Where(a => waterSourceIds.Contains(a.WaterSourceId))
                               .ProjectTo<AccessorApi.WaterSource>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.VariableSpecific>> GetVariables(HashSet<long> variableSpecificIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.VariablesDim
                               .Where(a => variableSpecificIds.Contains(a.VariableSpecificId))
                               .ProjectTo<AccessorApi.VariableSpecific>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.Method>> GetMethods(HashSet<long> methodIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.MethodsDim
                               .Where(a => methodIds.Contains(a.MethodId))
                               .ProjectTo<AccessorApi.Method>(Mapping.DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<List<SitesDim>> GetSites(HashSet<long> siteIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.SitesDim
                               .Where(a => siteIds.Contains(a.SiteId))
                               .ToListAsync();
            }
        }

        private async Task<List<PODSiteToPOUSiteFact>> GetPodPouSites(HashSet<long> sitesIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.PODSiteToPOUSiteFact
                               .Where(a => sitesIds.Any(b => b == a.PODSiteId) || sitesIds.Any(b => b == a.POUSiteId))
                               .Include(b => b.POUSite)
                               .Include(b => b.PODSite)
                               .ToListAsync();
            }
        }

        private async ValueTask<List<(long SiteVariableAmountId, BeneficialUsesCV BeneficialUse)>> GetBeneficialUses(HashSet<long> siteVariableAmountIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.SitesBridgeBeneficialUsesFact
                               .Where(a => siteVariableAmountIds.Contains(a.SiteVariableAmountId))
                               .Select(a => new { a.SiteVariableAmountId, a.BeneficialUse })
                               .AsAsyncEnumerable()
                               .Select(a => (a.SiteVariableAmountId, a.BeneficialUse))
                               .ToListAsync();
            }
        }

        private static void ProcessSiteVariableAmountsOrganization(AccessorApi.SiteVariableAmountsOrganization org, List<SiteVariableAmountHelper> results,
            List<AccessorApi.WaterSource> waterSources, List<AccessorApi.VariableSpecific> variableSpecifics, List<AccessorApi.Method> methods, List<(long SiteVariableAmountId, BeneficialUsesCV BeneficialUse)> beneficialUses, List<SitesDim> sites, List<PODSiteToPOUSiteFact> siteRelationships)
        {
            var allocations = results.Where(a => a.OrganizationId == org.OrganizationId).ToList();

            var siteVariableAmountsIds2 = allocations.Select(a => a.SiteVariableAmountId).ToHashSet();
            var waterSourceIds2 = allocations.Select(a => a.WaterSourceId).ToHashSet();
            var variableSpecificIds2 = allocations.Select(a => a.VariableSpecificId).ToHashSet();
            var methodIds2 = allocations.Select(a => a.MethodId).ToHashSet();
            var siteIds2 = allocations.Select(a => a.SiteID).ToHashSet();

            org.WaterSources = waterSources
                .Where(a => waterSourceIds2.Contains(a.WaterSourceId))
                .Map<List<AccessorApi.WaterSource>>();

            org.VariableSpecifics = variableSpecifics
                .Where(a => variableSpecificIds2.Contains(a.VariableSpecificId))
                .Map<List<AccessorApi.VariableSpecific>>();

            org.Methods = methods
                .Where(a => methodIds2.Contains(a.MethodId))
                .Map<List<AccessorApi.Method>>();

            org.BeneficialUses = beneficialUses
                .Where(a => siteVariableAmountsIds2.Contains(a.SiteVariableAmountId))
                .Select(a => a.BeneficialUse)
                .DistinctBy(a => a.Name)
                .Map<List<AccessorApi.BeneficialUse>>();

            org.Sites = sites
                .Where(a => siteIds2.Contains(a.SiteId))
                .Map<List<AccessorApi.Site>>();

            foreach (var site in org.Sites)
            {
                site.RelatedPODSites = siteRelationships.Where(a => a.POUSiteId == site.SiteID).Map<List<AccessorApi.PodToPouSiteRelationship>>(a => a.Items.Add(ApiProfile.PodPouKey, ApiProfile.PodValue));
                site.RelatedPOUSites = siteRelationships.Where(a => a.PODSiteId == site.SiteID).Map<List<AccessorApi.PodToPouSiteRelationship>>(a => a.Items.Add(ApiProfile.PodPouKey, ApiProfile.PouValue));
            }

            org.SiteVariableAmounts = allocations.Map<List<AccessorApi.SiteVariableAmount>>();

            foreach (var siteVariableAmount in org.SiteVariableAmounts)
            {
                siteVariableAmount.BeneficialUses = beneficialUses
                    .Where(a => a.SiteVariableAmountId == siteVariableAmount.SiteVariableAmountId)
                    .Select(a => a.BeneficialUse.Name).ToList();
            }
        }

        internal class SiteVariableAmountHelper
        {
            public long SiteVariableAmountId { get; set; }
            public string SiteName { get; set; }
            public string WaterSourceUUID { get; set; }
            public long SiteID { get; set; }
            public string NativeSiteID { get; set; }
            public string SiteTypeCV { get; set; }
            public double? Longitude { get; set; }
            public double? Latitude { get; set; }
            public Geometry SiteGeometry { get; set; }
            public string CoordinateMethodCV { get; set; }
            public string AllocationGNISIDCV { get; set; }
            public DateTime? TimeframeStart { get; set; }
            public DateTime? TimeframeEnd { get; set; }
            public DateTime? DataPublicationDate { get; set; }
            public double? AllocationCropDutyAmount { get; set; }
            public double? Amount { get; set; }
            public string IrrigationMethodCV { get; set; }
            public double? IrrigatedAcreage { get; set; }
            public string CropTypeCV { get; set; }
            public long? PopulationServed { get; set; }
            public double? PowerGeneratedGWh { get; set; }
            public string AllocationCommunityWaterSupplySystem { get; set; }
            public string SDWISIdentifier { get; set; }
            public string DataPublicationDOI { get; set; }
            public string ReportYearCV { get; set; }
            public string MethodUUID { get; set; }
            public string VariableSpecificTypeCV { get; set; }
            public string SiteUUID { get; set; }
            public string AssociatedNativeAllocationIDs { get; set; }
            public string HUC8 { get; set; }
            public string HUC12 { get; set; }
            public string County { get; set; }

            public long OrganizationId { get; set; }
            public long WaterSourceId { get; set; }
            public long VariableSpecificId { get; set; }
            public long MethodId { get; set; }
        }
    }
}
