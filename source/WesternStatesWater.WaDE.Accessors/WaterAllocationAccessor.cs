using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Concurrent;
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

        async Task<IEnumerable<AccessorApi.AllocationAmounts>> AccessorApi.IWaterAllocationAccessor.GetSiteAllocationAmountsAsync(string variableSpecificCV, string siteUuid, string beneficialUse)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                IQueryable<EntityFramework.AllocationAmountsFact> query = db.AllocationAmountsFact;
                if (!string.IsNullOrWhiteSpace(variableSpecificCV))
                {
                    query = query.Where(a => a.VariableSpecific.VariableSpecificCv == variableSpecificCV);
                }
                if (!string.IsNullOrWhiteSpace(siteUuid))
                {
                    query = query.Where(a => a.Site.SiteUuid == siteUuid);
                }
                if (!string.IsNullOrWhiteSpace(beneficialUse))
                {
                    query = query.Where(a => a.PrimaryBeneficialUse.BeneficialUseCategory == beneficialUse || a.AllocationBridgeBeneficialUsesFact.Any(b=>b.BeneficialUse.BeneficialUseCategory == beneficialUse));
                }

                return await query.ProjectTo<AccessorApi.AllocationAmounts>(Mapping.DtoMapper.Configuration).ToListAsync();
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadOrganizations(string runId, IEnumerable<AccessorImport.Organization> organizations)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadOrganization";

                cmd.CommandType = CommandType.StoredProcedure;

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

                return (int)resultParam.Value==1;
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadWaterAllocation(string runId, IEnumerable<AccessorImport.WaterAllocation> organizations)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "Core.LoadWaterAllocation";

                cmd.CommandType = CommandType.StoredProcedure;

                var runIdParam = new SqlParameter();
                runIdParam.ParameterName = "@RunId";
                runIdParam.Value = runId;
                cmd.Parameters.Add(runIdParam);

                var orgsParam = new SqlParameter();
                orgsParam.ParameterName = "@WaterAllocationTable";
                orgsParam.SqlDbType = SqlDbType.Structured;
                orgsParam.Value = organizations.Select(ConvertObjectToSqlDataRecords<AccessorImport.WaterAllocation>.Convert).ToList();
                orgsParam.TypeName = "Core.WaterAllocationTableType";
                cmd.Parameters.Add(orgsParam);

                var resultParam = new SqlParameter();
                resultParam.SqlDbType = SqlDbType.Bit;
                resultParam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(resultParam);

                await db.Database.OpenConnectionAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)resultParam.Value == 1;
            }
        }
    }

    internal static class ConvertObjectToSqlDataRecords<T>
    {
        private static PropertyInfo[] Properties = typeof(T).GetProperties();
        private static SqlMetaData[] TableSchema;
        static ConvertObjectToSqlDataRecords()
        {
            var tableSchema = new List<SqlMetaData>();
            foreach (var prop in Properties)
            {
                //todo: add support for other types
                if(prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.Date));
                }
                else if (prop.PropertyType == typeof(double))
                {
                    tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.Float));
                }
                else
                {
                    tableSchema.Add(new SqlMetaData(prop.Name, SqlDbType.NVarChar, 4000));
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
