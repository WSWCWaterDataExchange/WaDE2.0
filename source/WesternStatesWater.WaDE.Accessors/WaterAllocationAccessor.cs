using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Accessors
{
    public class WaterAllocationAccessor : AccessorApi.IWaterAllocationAccessor
    {
        public WaterAllocationAccessor(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<WaterAllocationAccessor>();
        }

        private ILogger Logger { get; }
        private IConfiguration Configuration { get; }

        async Task<AccessorApi.WaterAllocations> AccessorApi.IWaterAllocationAccessor.GetSiteAllocationAmountsAsync(AccessorApi.SiteAllocationAmountsFilters filters, int startIndex, int recordCount)
        {
            var sw = Stopwatch.StartNew();

            var totalCountTask = GetAllocationAmountsCount(filters).BlockTaskInTransaction();
            var results = await GetAllocationAmounts(filters, startIndex, recordCount).BlockTaskInTransaction();

            var allocationIds = results.Select(a => a.AllocationAmountId).ToHashSet();

            var sitesTask = GetSites(allocationIds).BlockTaskInTransaction();
            var beneficialUseTask = GetBeneficialUses(allocationIds).BlockTaskInTransaction();
            var orgsTask = GetOrganizations(results.Select(a => a.OrganizationId).ToHashSet()).BlockTaskInTransaction();
            var variableSpecificTask = GetVariables(results.Select(a => a.VariableSpecificId).ToHashSet()).BlockTaskInTransaction();
            var methodTask = GetMethods(results.Select(a => a.MethodId).ToHashSet()).BlockTaskInTransaction();

            var sites = await sitesTask;
            var sitesIds = sites.Select(a => a.Site.SiteId).ToHashSet();

            var waterSourceTask = GetWaterSources(sitesIds).BlockTaskInTransaction();
            var siteRelationshipsTask = GetPodPouSites(sitesIds).BlockTaskInTransaction();
            var regulatoryOverlayTask = GetRegulatoryOverlays(sitesIds).BlockTaskInTransaction();

            var beneficialUses = await beneficialUseTask;
            var waterSources = await waterSourceTask;
            var variableSpecifics = await variableSpecificTask;
            var methods = await methodTask;
            var regulatorOverlays = await regulatoryOverlayTask;
            var siteRelationships = await siteRelationshipsTask;

            var waterAllocationOrganizations = new List<AccessorApi.WaterAllocationOrganization>();
            foreach (var org in await orgsTask)
            {
                ProcessWaterAllocationOrganization(org, results, waterSources, variableSpecifics, methods, beneficialUses, sites, siteRelationships, regulatorOverlays);
                waterAllocationOrganizations.Add(org);
            }

            sw.Stop();
            Logger.LogInformation($"Completed WaterAllocation [{sw.ElapsedMilliseconds } ms]");
            return new AccessorApi.WaterAllocations
            {
                TotalWaterAllocationsCount = await totalCountTask,
                Organizations = waterAllocationOrganizations
            };
        }

        private static IQueryable<AllocationAmountsFact> BuildAllocationAmountsQuery(AccessorApi.SiteAllocationAmountsFilters filters, WaDEContext db)
        {
            var query = db.AllocationAmountsFact.AsNoTracking();

            if (filters.StartPriorityDate != null)
            {
                query = query.Where(a => a.AllocationPriorityDateNavigation.Date >= filters.StartPriorityDate);
            }
            if (filters.EndPriorityDate != null)
            {
                query = query.Where(a => a.AllocationPriorityDateNavigation.Date <= filters.EndPriorityDate);
            }
            if (!string.IsNullOrWhiteSpace(filters.SiteUuid))
            {
                query = query.Where(a => a.AllocationBridgeSitesFact.Any(s => s.Site.SiteUuid == filters.SiteUuid));
            }
            if (!string.IsNullOrWhiteSpace(filters.BeneficialUseCv))
            {
                query = query.Where(a => a.PrimaryUseCategoryCV == filters.BeneficialUseCv || a.AllocationBridgeBeneficialUsesFact.Any(b => b.BeneficialUseCV == filters.BeneficialUseCv));
            }
            if (!string.IsNullOrWhiteSpace(filters.UsgsCategoryNameCv))
            {
                query = query.Where(a => a.PrimaryBeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCv || a.AllocationBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCv));
            }
            if (filters.Geometry != null)
            {
                query = query.Where(
                    a => a.AllocationBridgeSitesFact.Any(site => site.Site.Geometry != null && site.Site.Geometry.Intersects(filters.Geometry)) || //
                         a.AllocationBridgeSitesFact.Any(site => site.Site.SitePoint != null && site.Site.SitePoint.Intersects(filters.Geometry))); //
            }
            if (!string.IsNullOrWhiteSpace(filters.HUC8))
            {
                query = query.Where(a => a.AllocationBridgeSitesFact.Any(b => b.Site.HUC8 == filters.HUC8));
            }
            if (!string.IsNullOrWhiteSpace(filters.HUC12))
            {
                query = query.Where(a => a.AllocationBridgeSitesFact.Any(b => b.Site.HUC12 == filters.HUC12));
            }
            if (!string.IsNullOrWhiteSpace(filters.County))
            {
                query = query.Where(a => a.AllocationBridgeSitesFact.Any(b => b.Site.County == filters.County));
            }
            if (!string.IsNullOrWhiteSpace(filters.State))
            {
                query = query.Where(a => a.Organization.State == filters.State);
            }

            return query;
        }

        private async Task<int> GetAllocationAmountsCount(AccessorApi.SiteAllocationAmountsFilters filters)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildAllocationAmountsQuery(filters, db).CountAsync();
            }
        }

        private async Task<List<AllocationHelper>> GetAllocationAmounts(AccessorApi.SiteAllocationAmountsFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildAllocationAmountsQuery(filters, db)
                                .OrderBy(a => a.AllocationAmountId)
                                .Skip(startIndex)
                                .Take(recordCount)
                                .ProjectTo<AllocationHelper>(Mapping.DtoMapper.Configuration)
                                .ToListAsync();
            }
        }

        private async ValueTask<List<(long AllocationAmountId, SitesDim Site)>> GetSites(HashSet<long> allocationIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.AllocationBridgeSitesFact
                               .Where(a => allocationIds.Contains(a.AllocationAmountId))
                               .Select(a => new { a.AllocationAmountId, a.Site })
                               .AsAsyncEnumerable()
                               .Select(a => (a.AllocationAmountId, a.Site))
                               .ToListAsync();
            }
        }

        private async ValueTask<List<(long AllocationAmountId, BeneficialUsesCV BeneficialUse)>> GetBeneficialUses(HashSet<long> allocationIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.AllocationBridgeBeneficialUsesFact
                               .Where(a => allocationIds.Contains(a.AllocationAmountId))
                               .Select(a => new { a.AllocationAmountId, a.BeneficialUse })
                               .AsAsyncEnumerable()
                               .Select(a => (a.AllocationAmountId, a.BeneficialUse))
                               .ToListAsync();
            }
        }

        private async Task<List<AccessorApi.WaterAllocationOrganization>> GetOrganizations(HashSet<long> orgIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.OrganizationsDim
                               .Where(a => orgIds.Contains(a.OrganizationId))
                               .ProjectTo<AccessorApi.WaterAllocationOrganization>(Mapping.DtoMapper.Configuration)
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

        private async Task<List<AccessorApi.RegulatoryOverlay>> GetRegulatoryOverlays(HashSet<long> sitesIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await db.RegulatoryOverlayBridgeSitesFact
                               .Where(a => sitesIds.Contains(a.SiteId))
                               .Select(a => a.RegulatoryOverlay)
                               .ProjectTo<AccessorApi.RegulatoryOverlay>(DtoMapper.Configuration)
                               .ToListAsync();
            }
        }

        private async Task<Dictionary<long, List<WaterSourcesDim>>> GetWaterSources(HashSet<long> sitesIds)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var results = await db.WaterSourceBridgeSitesFact
                                      .Where(a => sitesIds.Contains(a.SiteId))
                                      .Select(a => new { a.SiteId, a.WaterSource })
                                      .ToListAsync();
                return results.GroupBy(a => a.SiteId)
                              .ToDictionary(a => a.Key, b => b.Select(c => c.WaterSource).ToList());
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

        async Task<IEnumerable<AccessorApi.WaterAllocationsDigest>> AccessorApi.IWaterAllocationAccessor.GetSiteAllocationAmountsDigestAsync(AccessorApi.SiteAllocationAmountsDigestFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var sw = Stopwatch.StartNew();
                var results = await GetAllocationAmounts(filters, startIndex, recordCount).BlockTaskInTransaction();

                var sitesTask = GetSites(results.Select(a => a.AllocationAmountId).ToHashSet()).BlockTaskInTransaction();

                var sites = await sitesTask;
                var waterAllocationsLight = new List<AccessorApi.WaterAllocationsDigest>();
                foreach (var allocationAmounts in results)
                {
                    var record = new AccessorApi.WaterAllocationsDigest
                    {
                        AllocationAmountId = allocationAmounts.AllocationAmountId,
                        AllocationFlow_CFS = allocationAmounts.AllocationFlow_CFS,
                        AllocationVolume_AF = allocationAmounts.AllocationVolume_AF,
                        AllocationPriorityDate = allocationAmounts.AllocationPriorityDate
                    };

                    var sights = new List<AccessorApi.SiteDigest>();
                    sights.AddRange(sites.Where(x => x.AllocationAmountId == allocationAmounts.AllocationAmountId)
                        .Select(x => new AccessorApi.SiteDigest
                        {
                            Latitude = x.Site.Latitude,
                            Longitude = x.Site.Longitude,
                            SiteUUID = x.Site.SiteUuid
                        }));

                    record.Sites = sights;
                    waterAllocationsLight.Add(record);
                }

                sw.Stop();
                Logger.LogInformation($"Completed WaterAllocationLight [{sw.ElapsedMilliseconds } ms]");
                return waterAllocationsLight;
            }
        }

        private static IQueryable<AllocationAmountsFact> BuildAllocationAmountsDigestQuery(AccessorApi.SiteAllocationAmountsDigestFilters filters, WaDEContext db)
        {
            var query = db.AllocationAmountsFact.AsNoTracking();

            if (filters.StartPriorityDate != null)
            {
                query = query.Where(a => a.AllocationPriorityDateNavigation.Date >= filters.StartPriorityDate);
            }
            if (filters.EndPriorityDate != null)
            {
                query = query.Where(a => a.AllocationPriorityDateNavigation.Date <= filters.EndPriorityDate);
            }
            if (!string.IsNullOrWhiteSpace(filters.BeneficialUseCv))
            {
                query = query.Where(a => a.PrimaryUseCategoryCV == filters.BeneficialUseCv || a.AllocationBridgeBeneficialUsesFact.Any(b => b.BeneficialUseCV == filters.BeneficialUseCv));
            }
            if (!string.IsNullOrWhiteSpace(filters.UsgsCategoryNameCv))
            {
                query = query.Where(a => a.PrimaryBeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCv || a.AllocationBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCv));
            }
            if (filters.Geometry != null)
            {
                query = query.Where(
                    a => a.AllocationBridgeSitesFact.Any(site => site.Site.Geometry != null && site.Site.Geometry.Intersects(filters.Geometry)) ||
                    a.AllocationBridgeSitesFact.Any(site => site.Site.SitePoint != null && site.Site.SitePoint.Intersects(filters.Geometry)));
            }
            if (!string.IsNullOrWhiteSpace(filters.OrganizationUUID))
            {
                query = query.Where(a => a.Organization.OrganizationUuid == filters.OrganizationUUID);
            }

            return query;
        }

        private async Task<List<AllocationHelper>> GetAllocationAmounts(AccessorApi.SiteAllocationAmountsDigestFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                return await BuildAllocationAmountsDigestQuery(filters, db)
                                .OrderBy(a => a.AllocationAmountId)
                                .Skip(startIndex)
                                .Take(recordCount)
                                .ProjectTo<AllocationHelper>(Mapping.DtoMapper.Configuration)
                                .ToListAsync();
            }
        }

        private static void ProcessWaterAllocationOrganization(AccessorApi.WaterAllocationOrganization org,
            List<AllocationHelper> results,
            Dictionary<long, List<WaterSourcesDim>> waterSources,
            List<AccessorApi.VariableSpecific> variableSpecifics,
            List<AccessorApi.Method> methods,
            List<(long AllocationAmountId, BeneficialUsesCV BeneficialUse)> beneficialUses,
            List<(long AllocationAmountId, SitesDim Site)> sites,
            List<PODSiteToPOUSiteFact> siteRelationships,
            List<AccessorApi.RegulatoryOverlay> regulatoryOverlays
            )
        {
            var allocations = results.Where(a => a.OrganizationId == org.OrganizationId).ToList();

            var allocationIds = allocations.Select(a => a.AllocationAmountId).ToHashSet();
            var variableSpecificIds = allocations.Select(a => a.VariableSpecificId).ToHashSet();
            var methodIds = allocations.Select(a => a.MethodId).ToHashSet();

            org.VariableSpecifics = variableSpecifics
            .Where(a => variableSpecificIds.Contains(a.VariableSpecificId))
            .Map<List<AccessorApi.VariableSpecific>>();

            org.Methods = methods
                .Where(a => methodIds.Contains(a.MethodId))
                .Map<List<AccessorApi.Method>>();

            org.BeneficialUses = beneficialUses
                .Where(a => allocationIds.Contains(a.AllocationAmountId))
                .Select(a => a.BeneficialUse)
                .DistinctBy(a => a.Name)
                .Map<List<AccessorApi.BeneficialUse>>();

            org.RegulatoryOverlays = regulatoryOverlays;

            org.WaterAllocations = allocations.Map<List<AccessorApi.Allocation>>();

            foreach (var site in sites.Select(a => a.Site))
            {
                site.PODSiteToPOUSitePODFact = siteRelationships.Where(a => a.PODSiteId == site.SiteId).ToList();
                site.PODSiteToPOUSitePOUFact = siteRelationships.Where(a => a.POUSiteId == site.SiteId).ToList();
            }

            var waterSourceUuids = new HashSet<string>();
            foreach (var waterAllocation in org.WaterAllocations)
            {
                waterAllocation.BeneficialUses = beneficialUses
                    .Where(a => a.AllocationAmountId == waterAllocation.AllocationAmountId)
                    .Select(a => a.BeneficialUse.Name).ToList();

                waterAllocation.Sites = sites
                    .Where(a => a.AllocationAmountId == waterAllocation.AllocationAmountId)
                    .Select(a => a.Site)
                    .Map<List<AccessorApi.Site>>();

                foreach (var site in waterAllocation.Sites)
                {
                    waterSources.TryGetValue(site.SiteID, out var waterSources1);
                    if (waterSources1 != null)
                    {
                        site.WaterSourceUUIDs = waterSources1.Select(a => a.WaterSourceUuid).ToList();
                        foreach (var waterSourceUuid in site.WaterSourceUUIDs)
                        {
                            waterSourceUuids.Add(waterSourceUuid);
                        }
                    }
                    else
                    {
                        site.WaterSourceUUIDs = new List<string>();
                    }
                }
            }

            org.WaterSources = waterSources.SelectMany(a => a.Value).Where(a => waterSourceUuids.Contains(a.WaterSourceUuid)).DistinctBy(a => a.WaterSourceUuid).Map<List<AccessorApi.WaterSource>>();
        }

        internal class AllocationHelper
        {
            public long OrganizationId { get; set; }
            public long AllocationAmountId { get; set; }
            public string AllocationNativeID { get; set; }
            public string AllocationOwner { get; set; }
            public DateTime? AllocationApplicationDate { get; set; }
            public DateTime? AllocationPriorityDate { get; set; }
            public string AllocationLegalStatusCodeCV { get; set; }
            public DateTime? AllocationExpirationDate { get; set; }
            public string AllocationChangeApplicationIndicator { get; set; }
            public string LegacyAllocationIDs { get; set; }
            public double? AllocationAcreage { get; set; }
            public string AllocationBasisCV { get; set; }
            public DateTime? TimeframeStart { get; set; }
            public DateTime? TimeframeEnd { get; set; }
            public DateTime? DataPublicationDate { get; set; }
            public double? AllocationCropDutyAmount { get; set; }
            public double? AllocationFlow_CFS { get; set; }
            public double? AllocationVolume_AF { get; set; }
            public long? PopulationServed { get; set; }
            public double? GeneratedPowerCapacityMW { get; set; }
            public string AllocationCommunityWaterSupplySystem { get; set; }
            public string AllocationSDWISIdentifier { get; set; }
            public long MethodId { get; set; }
            public string MethodUUID { get; set; }
            public long VariableSpecificId { get; set; }
            public string VariableSpecificTypeCV { get; set; }
            public string PrimaryUseCategoryCV { get; set; }
            public bool ExemptOfVolumeFlowPriority { get; set; }
        }
    }
}
