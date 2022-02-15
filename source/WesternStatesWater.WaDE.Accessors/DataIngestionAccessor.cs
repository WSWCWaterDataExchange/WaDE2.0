using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Accessors
{
    public class DataIngestionAccessor : AccessorImport.IDataIngestionAccessor
    {
        public DataIngestionAccessor(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<WaterAllocationAccessor>();
        }

        private ILogger Logger { get; }
        private IConfiguration Configuration { get; }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadOrganizations(string runId, IEnumerable<AccessorImport.Organization> organizations)
        {
            return await LoadData(runId, organizations, "LoadOrganization", "@OrganizationTable", "OrganizationTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadWaterAllocation(string runId, IEnumerable<AccessorImport.WaterAllocation> waterAllocations)
        {
            return await LoadData(runId, waterAllocations, "LoadWaterAllocation", "@WaterAllocationTable", "WaterAllocationTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadAggregatedAmounts(string runId, IEnumerable<AccessorImport.AggregatedAmount> aggregatedAmounts)
        {
            return await LoadData(runId, aggregatedAmounts, "LoadAggregatedAmounts", "@AggregatedAmountTable", "AggregatedAmountTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadMethods(string runId, IEnumerable<AccessorImport.Method> methods)
        {
            return await LoadData(runId, methods, "LoadMethods", "@MethodTable", "MethodTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadRegulatoryOverlays(string runId, IEnumerable<AccessorImport.RegulatoryOverlay> regulatoryOverlays)
        {
            return await LoadData(runId, regulatoryOverlays, "LoadRegulatoryOverlays", "@RegulatoryOverlayTable", "RegulatoryOverlayTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadRegulatoryReportingUnits(string runId, IEnumerable<AccessorImport.RegulatoryReportingUnits> loadRegulatoryReportingUnits)
        {
            return await LoadData(runId, loadRegulatoryReportingUnits, "LoadRegulatoryReportingUnits", "@RegulatoryReportingUnitsTableType", "RegulatoryReportingUnitsTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadReportingUnits(string runId, IEnumerable<AccessorImport.ReportingUnit> reportingUnits)
        {
            return await LoadData(runId, reportingUnits, "LoadReportingUnits", "@ReportingUnitTable", "ReportingUnitTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadSites(string runId, IEnumerable<AccessorImport.Site> sites)
        {
            return await LoadData(runId, sites, "LoadSites", "@SiteTable", "SiteTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadSiteSpecificAmounts(string runId, IEnumerable<AccessorImport.SiteSpecificAmount> siteSpecificAmounts)
        {
            return await LoadData(runId, siteSpecificAmounts, "LoadSiteSpecificAmounts", "@SiteSpecificAmountTable", "SiteSpecificAmountTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadVariables(string runId, IEnumerable<AccessorImport.Variable> variables)
        {
            return await LoadData(runId, variables, "LoadVariables", "@VariableTable", "VariableTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadWaterSources(string runId, IEnumerable<AccessorImport.WaterSource> waterSources)
        {
            return await LoadData(runId, waterSources, "LoadWaterSources", "@WaterSourceTable", "WaterSourceTableType");
        }

        async Task<bool> AccessorImport.IDataIngestionAccessor.LoadPodSitePouSiteFact(string runId, IEnumerable<AccessorImport.PODSitePOUSite> podSitePouSiteFacts)
        {
            return await LoadData(runId, podSitePouSiteFacts, "LoadPODSitePOUSiteFacts", "@PODSitePOUSiteFactTable", "PODSitePOUSiteFactTableType");
        }

        private async Task<bool> LoadData<T>(string runId, IEnumerable<T> records, string importStoredProcedureName, string tableParameterName, string tableTypeName)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("WadeDatabase")))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"Core.{importStoredProcedureName}";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;

                var runIdParam = new SqlParameter();
                runIdParam.ParameterName = "@RunId";
                runIdParam.Value = runId;
                cmd.Parameters.Add(runIdParam);

                var orgsParam = new SqlParameter();
                orgsParam.ParameterName = tableParameterName;
                orgsParam.SqlDbType = SqlDbType.Structured;
                orgsParam.Value = records.Select(ConvertObjectToSqlDataRecords<T>.Convert).ToList();
                orgsParam.TypeName = $"Core.{tableTypeName}";
                cmd.Parameters.Add(orgsParam);

                var resultParam = new SqlParameter();
                resultParam.SqlDbType = SqlDbType.Bit;
                resultParam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(resultParam);

                await connection.OpenAsync().BlockTaskInTransaction();
                await cmd.ExecuteNonQueryAsync().BlockTaskInTransaction();

                return (int)resultParam.Value == 0;
            }
        }

        private static class ConvertObjectToSqlDataRecords<T>
        {
            private static readonly PropertyInfo[] Properties = typeof(T).GetProperties();
            private static readonly SqlMetaData[] TableSchema;
            static ConvertObjectToSqlDataRecords()
            {
                var tableSchema = new List<Microsoft.Data.SqlClient.Server.SqlMetaData>();
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
