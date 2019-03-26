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

        async Task<IEnumerable<AccessorApi.AllocationAmounts>> AccessorApi.IWaterAllocationAccessor.GetSiteAllocationAmountsAsync(string variableSpecificCV, string siteUuid)
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

                return await query.ProjectTo<AccessorApi.AllocationAmounts>(Mapping.DtoMapper.Configuration).ToListAsync();
            }
        }

        async Task<bool> AccessorImport.IWaterAllocationAccessor.LoadOrganizations(string runId, IEnumerable<AccessorImport.Organization> organizations)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "exec Core.LoadOrganization @runId @orgs @result";

                var orgsParam = new SqlParameter();
                orgsParam.ParameterName = "@orgs";
                orgsParam.SqlDbType = SqlDbType.Structured;
                orgsParam.Value = organizations.Select(ConvertObjectToSqlDataRecords<AccessorImport.Organization>.Convert);
                orgsParam.TypeName = "Core.OrganizationTableType";
                cmd.Parameters.Add(orgsParam);

                var runIdParam = new SqlParameter();
                runIdParam.ParameterName = "@runId";
                runIdParam.Value = runId;
                cmd.Parameters.Add(runIdParam);

                var resultParam = new SqlParameter();
                resultParam.ParameterName = "@result";
                resultParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(resultParam);

                await cmd.ExecuteNonQueryAsync();

                return (bool)resultParam.Value;
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
                var type = SqlDbType.NVarChar; //todo: add support for other types
                tableSchema.Add(new SqlMetaData(prop.Name, type));
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
