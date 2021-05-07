using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using MoreLinq;
using NetTopologySuite;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Accessors.Mapping;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;
using System.Data.Sql;

namespace WesternStatesWater.WaDE.Accessors
{
    public class WaterAllocationAccessor : AccessorApi.IWaterAllocationAccessor, AccessorImport.IWaterAllocationAccessor
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
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var sw = Stopwatch.StartNew();
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
                if (!string.IsNullOrWhiteSpace(filters.Geometry))
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    WKTReader reader = new WKTReader(geometryFactory);
                    var shape = reader.Read(filters.Geometry);
                    query = query.Where(
                        a => a.AllocationBridgeSitesFact.Any(site => site.Site.Geometry != null && site.Site.Geometry.Intersects(shape)) ||
                        a.AllocationBridgeSitesFact.Any(site => site.Site.SitePoint != null && site.Site.SitePoint.Intersects(shape)));
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

                var totalCount = query.Count();

                var results = await query
                    .OrderBy(a => a.AllocationAmountId)
                    .Skip(startIndex)
                    .Take(recordCount)
                    .ProjectTo<AllocationHelper>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var allocationIds = results.Select(a => a.AllocationAmountId).ToList();

                var sitesTask = db.AllocationBridgeSitesFact
                    .Where(a => allocationIds.Contains(a.AllocationAmountId))
                    .Select(a => new { a.AllocationAmountId, a.Site })
                    .ToListAsync();

                var beneficialUseTask = db.AllocationBridgeBeneficialUsesFact
                    .Where(a => allocationIds.Contains(a.AllocationAmountId))
                    .Select(a => new { a.AllocationAmountId, a.BeneficialUse })
                    .ToListAsync();

                var orgIds = results.Select(a => a.OrganizationId).ToHashSet();
                var orgsTask = db.OrganizationsDim
                    .Where(a => orgIds.Contains(a.OrganizationId))
                    .ProjectTo<AccessorApi.WaterAllocationOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var variableSpecificIds = results.Select(a => a.VariableSpecificId).ToHashSet();
                var variableSpecificTask = db.VariablesDim
                    .Where(a => variableSpecificIds.Contains(a.VariableSpecificId))
                    .ProjectTo<AccessorApi.VariableSpecific>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var methodIds = results.Select(a => a.MethodId).ToHashSet();
                var methodTask = db.MethodsDim
                    .Where(a => methodIds.Contains(a.MethodId))
                    .ProjectTo<AccessorApi.Method>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var sites = (await sitesTask).Select(a => (a.AllocationAmountId, a.Site)).ToList();
                
                var waterSourceIds = sites.Select(a => a.Site.WaterSourceId).ToHashSet();
                var waterSourceTask = db.WaterSourcesDim
                    .Where(a => waterSourceIds.Contains(a.WaterSourceId))
                    .ProjectTo<AccessorApi.WaterSource>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();
                
                var beneficialUses = (await beneficialUseTask).Select(a => (a.AllocationAmountId, a.BeneficialUse)).ToList();
                var waterSources = await waterSourceTask;
                var variableSpecifics = await variableSpecificTask;
                var methods = await methodTask;

                var waterAllocationOrganizations = new List<AccessorApi.WaterAllocationOrganization>();
                foreach (var org in await orgsTask)
                {
                    ProcessWaterAllocationOrganization(org, results, waterSources, variableSpecifics, methods, beneficialUses, sites);
                    waterAllocationOrganizations.Add(org);
                }

                sw.Stop();
                Logger.LogInformation($"Completed WaterAllocation [{sw.ElapsedMilliseconds } ms]");
                return new AccessorApi.WaterAllocations
                {
                    TotalWaterAllocationsCount = totalCount,
                    Organizations = waterAllocationOrganizations
                };
            }
        }

        async Task<IEnumerable<AccessorApi.WaterAllocationsDigest>> AccessorApi.IWaterAllocationAccessor.GetSiteAllocationAmountsDigestAsync(AccessorApi.SiteAllocationAmountsDigestFilters filters, int startIndex, int recordCount)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var sw = Stopwatch.StartNew();
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
                if (!string.IsNullOrWhiteSpace(filters.Geometry))
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    WKTReader reader = new WKTReader(geometryFactory);
                    var shape = reader.Read(filters.Geometry);
                    query = query.Where(
                        a => a.AllocationBridgeSitesFact.Any(site => site.Site.Geometry != null && site.Site.Geometry.Intersects(shape)) ||
                        a.AllocationBridgeSitesFact.Any(site => site.Site.SitePoint != null && site.Site.SitePoint.Intersects(shape)));
                }
                if (!string.IsNullOrWhiteSpace(filters.OrganizationUUID))
                {
                    query = query.Where(a => a.Organization.OrganizationUuid == filters.OrganizationUUID);
                }

                var results = await query
                    .OrderBy(a => a.AllocationAmountId)
                    .Skip(startIndex)
                    .Take(recordCount)
                    .ProjectTo<AllocationHelper>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var allocationIds = results.Select(a => a.AllocationAmountId).ToList();

                var sitesTask = db.AllocationBridgeSitesFact
                    .Where(a => allocationIds.Contains(a.AllocationAmountId))
                    .Select(a => new { a.AllocationAmountId, a.Site })
                    .ToListAsync();

                var sites = (await sitesTask).Select(a => (a.AllocationAmountId, a.Site)).ToList();
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

        private static void ProcessWaterAllocationOrganization(AccessorApi.WaterAllocationOrganization org,
            List<AllocationHelper> results,
            List<AccessorApi.WaterSource> waterSources,
            List<AccessorApi.VariableSpecific> variableSpecifics,
            List<AccessorApi.Method> methods,
            List<(long AllocationAmountId, BeneficialUsesCV BeneficialUse)> beneficialUses,
            List<(long AllocationAmountId, SitesDim Site)> sites)
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

            org.WaterAllocations = allocations.Map<List<AccessorApi.Allocation>>();

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
                    site.WaterSourceUUID = waterSources.First(a => a.WaterSourceId == site.WaterSourceId).WaterSourceUUID;
                }
            }

            var waterSourceIds = org.WaterAllocations.SelectMany(a => a.Sites.Select(b => b.WaterSourceId)).ToHashSet();
            org.WaterSources = waterSources.Where(a => waterSourceIds.Contains(a.WaterSourceId)).ToList();
            
            
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

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadOrganizations(string runId, IEnumerable<AccessorImport.Organization> organizations)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadOrganization";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter();
                runIdParam.ParameterName = "@RunId";
                runIdParam.Value = runId;
                cmd.Parameters.Add(runIdParam);

                var orgsParam = new SqlParameter();
                orgsParam.ParameterName = "@OrganizationTable";
                orgsParam.SqlDbType = SqlDbType.Structured;
                orgsParam.Value = organizations.Select(ConvertObjectToSqlDataRecords<AccessorImport.Organization>.Convert).ToList();
                orgsParam.TypeName = "Core.OrganizationTableType";
                cmd.Parameters.Add(orgsParam);

                var resultParam = new SqlParameter();
                resultParam.SqlDbType = SqlDbType.Bit;
                resultParam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadWaterAllocation(string runId, IEnumerable<AccessorImport.WaterAllocation> waterAllocations)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadWaterAllocation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter();
                runIdParam.ParameterName = "@RunId";
                runIdParam.Value = runId;
                cmd.Parameters.Add(runIdParam);

                var orgsParam = new SqlParameter();
                orgsParam.ParameterName = "@WaterAllocationTable";
                orgsParam.SqlDbType = SqlDbType.Structured;
                orgsParam.Value = waterAllocations.Select(ConvertObjectToSqlDataRecords<AccessorImport.WaterAllocation>.Convert).ToList();
                orgsParam.TypeName = "Core.WaterAllocationTableType";
                cmd.Parameters.Add(orgsParam);

                var resultParam = new SqlParameter();
                resultParam.SqlDbType = SqlDbType.Bit;
                resultParam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadAggregatedAmounts(string runId, IEnumerable<AccessorImport.AggregatedAmount> aggregatedAmounts)
        {
            var count = aggregatedAmounts.Where(a => string.IsNullOrWhiteSpace(a.PrimaryUseCategory)).ToArray();

            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadAggregatedAmounts";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var amountsParam = new SqlParameter
                {
                    ParameterName = "@AggregatedAmountTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = aggregatedAmounts.Select(ConvertObjectToSqlDataRecords<AccessorImport.AggregatedAmount>.Convert).ToList(),
                    TypeName = "Core.AggregatedAmountTableType"
                };

                cmd.Parameters.Add(amountsParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadMethods(string runId, IEnumerable<AccessorImport.Method> methods)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadMethods";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var methodsParam = new SqlParameter
                {
                    ParameterName = "@MethodTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = methods.Select(ConvertObjectToSqlDataRecords<AccessorImport.Method>.Convert).ToList(),
                    TypeName = "Core.MethodTableType"
                };

                cmd.Parameters.Add(methodsParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadRegulatoryOverlays(string runId, IEnumerable<AccessorImport.RegulatoryOverlay> regulatoryOverlays)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadRegulatoryOverlays";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var regulatoryParam = new SqlParameter
                {
                    ParameterName = "@RegulatoryOverlayTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = regulatoryOverlays.Select(ConvertObjectToSqlDataRecords<AccessorImport.RegulatoryOverlay>.Convert).ToList(),
                    TypeName = "Core.RegulatoryOverlayTableType"
                };

                cmd.Parameters.Add(regulatoryParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadRegulatoryReportingUnits(string runId, IEnumerable<AccessorImport.RegulatoryReportingUnits> LoadRegulatoryReportingUnits)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadRegulatoryReportingUnits";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var regulatoryParam = new SqlParameter
                {
                    ParameterName = "@RegulatoryReportingUnitsTableType",
                    SqlDbType = SqlDbType.Structured,
                    Value = LoadRegulatoryReportingUnits.Select(ConvertObjectToSqlDataRecords<AccessorImport.RegulatoryReportingUnits>.Convert).ToList(),
                    TypeName = "Core.RegulatoryReportingUnitsTableType"
                };

                cmd.Parameters.Add(regulatoryParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadReportingUnits(string runId, IEnumerable<AccessorImport.ReportingUnit> reportingUnits)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadReportingUnits";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var regulatoryParam = new SqlParameter
                {
                    ParameterName = "@ReportingUnitTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = reportingUnits.Select(ConvertObjectToSqlDataRecords<AccessorImport.ReportingUnit>.Convert).ToList(),
                    TypeName = "Core.ReportingUnitTableType"
                };

                cmd.Parameters.Add(regulatoryParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadSites(string runId, IEnumerable<AccessorImport.Site> sites)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadSites";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var siteParam = new SqlParameter
                {
                    ParameterName = "@SiteTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = sites.Select(ConvertObjectToSqlDataRecords<AccessorImport.Site>.Convert).ToList(),
                    TypeName = "Core.SiteTableType"
                };

                cmd.Parameters.Add(siteParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadSiteSpecificAmounts(string runId, IEnumerable<AccessorImport.SiteSpecificAmount> siteSpecificAmounts)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadSiteSpecificAmounts";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var amountParam = new SqlParameter
                {
                    ParameterName = "@SiteSpecificAmountTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = siteSpecificAmounts.Select(ConvertObjectToSqlDataRecords<AccessorImport.SiteSpecificAmount>.Convert).ToList(),
                    TypeName = "Core.SiteSpecificAmountTableType"
                };

                cmd.Parameters.Add(amountParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadVariables(string runId, IEnumerable<AccessorImport.Variable> variables)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadVariables";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var amountParam = new SqlParameter
                {
                    ParameterName = "@VariableTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = variables.Select(ConvertObjectToSqlDataRecords<AccessorImport.Variable>.Convert).ToList(),
                    TypeName = "Core.VariableTableType"
                };

                cmd.Parameters.Add(amountParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadWaterSources(string runId, IEnumerable<AccessorImport.WaterSource> waterSources)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadWaterSources";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter
                {
                    ParameterName = "@RunId",
                    Value = runId
                };

                cmd.Parameters.Add(runIdParam);

                var amountParam = new SqlParameter
                {
                    ParameterName = "@WaterSourceTable",
                    SqlDbType = SqlDbType.Structured,
                    Value = waterSources.Select(ConvertObjectToSqlDataRecords<AccessorImport.WaterSource>.Convert).ToList(),
                    TypeName = "Core.WaterSourceTableType"
                };

                cmd.Parameters.Add(amountParam);

                var resultParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 0;
            }
        }

        private static class ConvertObjectToSqlDataRecords<T>
        {
            private static PropertyInfo[] Properties = typeof(T).GetProperties();
            private static SqlMetaData[] TableSchema;
            static ConvertObjectToSqlDataRecords()
            {
                var tableSchema = new List<SqlMetaData>();
                foreach (var prop in Properties)
                {
                    //todo: add support for other types
                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.Date));
                    }
                    else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                    {
                        tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.Float));
                    }
                    else if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
                    {
                        tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.BigInt));
                    }
                    else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                    {
                        tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.Bit));
                    }
                    else
                    {
                        tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.NVarChar, -1));
                    }
                }
                TableSchema = tableSchema.ToArray();
            }

            public static SqlDataRecord Convert(T obj)
            {
                var tableRow = new SqlDataRecord(TableSchema);
                for (int i = 0; i < Properties.Length; i++)
                {
                    tableRow.SetValue(i, Properties[i].GetGetMethod().Invoke(obj, new object[0]));
                }
                return tableRow;
            }
        }
    }
}
