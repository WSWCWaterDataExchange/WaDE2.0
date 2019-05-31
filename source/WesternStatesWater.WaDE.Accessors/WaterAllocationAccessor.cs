using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using NetTopologySuite;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Accessors
{
    public class WaterAllocationAccessor : AccessorApi.IWaterAllocationAccessor, AccessorImport.IWaterAllocationAccessor
    {
        public WaterAllocationAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        async Task<IEnumerable<AccessorApi.WaterAllocationOrganization>> AccessorApi.IWaterAllocationAccessor.GetSiteAllocationAmountsAsync(string siteUuid, string beneficialUse, string geometry, DateTime? startPriorityDate, DateTime? endPriorityDate)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {

                var query = db.AllocationAmountsFact
                    .AsNoTracking();
                if (startPriorityDate != null)
                {
                    query = query.Where(a => a.AllocationPriorityDateNavigation.Date >= startPriorityDate);
                }
                if (endPriorityDate != null)
                {
                    query = query.Where(a => a.AllocationPriorityDateNavigation.Date <= endPriorityDate);
                }
                if (!string.IsNullOrWhiteSpace(siteUuid))
                {
                    query = query.Where(a => a.Site.SiteUuid == siteUuid);
                }
                if (!string.IsNullOrWhiteSpace(beneficialUse))
                {
                    query = query.Where(a => a.PrimaryBeneficialUse.BeneficialUseCategory == beneficialUse || a.AllocationBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.BeneficialUseCategory == beneficialUse));
                }
                if (!string.IsNullOrWhiteSpace(geometry))
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    WKTReader reader = new WKTReader(geometryFactory);
                    var shape = reader.Read(geometry);
                    query = query.Where(a => a.Site.Geometry != null && shape.Covers(a.Site.Geometry));
                }

                return await query
                    .GroupBy(a => a.Organization)
                    .ProjectTo<AccessorApi.WaterAllocationOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();
            }
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
