using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Common;

namespace WaDEImportFunctions
{
    public class CvData
    {
        public CvData(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string Consumptive = "Consumptive";
        private const string NonConsumptive = "Non-consumptive";

        private IConfiguration Configuration { get; set; }

        [FunctionName("CvDataUpdate")]
        public async Task Update([QueueTrigger("cv-data-update", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            //This is a quick and dirty process.  It should be converted at some point to a proper call chain (or moved into a working azure data factory process)
            log.LogInformation($"Updating All CV Data");

            var cvData = new List<(string Name, string Table)>()
            {
                ("aggregationstatistic", "AggregationStatistic"),
                ("reportyeartype", "ReportYearType"),
                ("epsgcode", "EPSGCode"),
                ("gnisfeaturename", "gnisfeaturename"),
                ("croptype", "croptype"),
                ("irrigationmethod", "irrigationmethod"),
                ("legalstatus", "legalstatus"),
                ("methodtype", "methodtype"),
                ("nhdnetworkstatus", "nhdnetworkstatus"),
                ("nhdproduct", "nhdproduct"),
                ("regulatorystatus", "regulatorystatus"),
                ("reportingunittype", "reportingunittype"),
                ("reportyear", "reportyearcv"),
                ("sitetype", "sitetype"),
                ("units", "units"),
                ("variable", "variable"),
                ("variablespecific", "variablespecific"),
                ("waterallocationbasis", "waterallocationbasis"),
                ("waterqualityindicator", "waterqualityindicator"),
                ("waterallocationtype", "waterallocationtype"),
                ("watersourcetype", "watersourcetype"),
                ("applicableresourcetype", "applicableresourcetype"),
                ("coordinatemethod", "coordinatemethod"),
                ("beneficialusecategory", "BeneficialUses"),
                ("sdwisidentifier", "SDWISIdentifier"),
                ("powertype", "PowerType"),
                ("states", "State"),
                ("regulatoryoverlaytype", "RegulatoryOverlayType"),
                ("ownerclassification", "OwnerClassification")
            };
            await Task.WhenAll(cvData.Select(a => ProcessCvTable(a.Name, a.Table, log)));

            log.LogInformation("Completed Updating All CV Data");
        }

        private async Task ProcessCvTable(string name, string table, ILogger log)
        {
            var data = await FetchData(name);
            var records = ParseData(data);
            await LoadData(table, records, log);
        }

        private async Task<string> FetchData(string name)
        {
            return await new HttpClient().GetStringAsync($"{Configuration.GetValue<string>("Endpoints:Vocabulary")}/{name}/?format=csv");
        }

        private List<dynamic> ParseData(string data)
        {
            using (var reader = new CsvHelper.CsvReader(new StringReader(data)))
            {
                return reader.GetRecords<dynamic>().ToList();
            }
        }

        private async Task LoadData(string table, List<dynamic> data, ILogger log)
        {

            log.LogInformation($"Updating CV Table [{table}]");
            using (var db = new WaDEContext(Configuration))
            using (var ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }, TransactionScopeAsyncFlowOption.Enabled))
            {
                //This method is very slow doing this one record at a time.  It should be updated to be able to do multiple records in one command.
                foreach (var record in data)
                {
                    var name = table == "reportyearcv" ? record.name.Substring(0, 4) : record.name;
                    try
                    {
                        if (table == "BeneficialUses")
                        {
                            var sql = $@"MERGE CVs.{table} AS target
    USING (SELECT @p0 Term, @p1 Name, @p2 State, @p3 Source, @p4 Def, @p5 WaDEName, @p6 ConsumptionCategoryType) AS source
        ON target.Name = source.Name
    WHEN MATCHED THEN UPDATE
        SET term = source.term,
            state = source.state,
            sourceVocabularyURI = source.source,
            definition = source.def,
            WaDEName = source.wadename,
            consumptionCategoryType = source.consumptioncategorytype
    WHEN NOT MATCHED THEN 
        INSERT (name, term, state, sourceVocabularyURI, definition, wadename, ConsumptionCategoryType) 
        VALUES (source.name, source.term, source.state, source.source, source.def, source.wadename, source.consumptionCategoryType);";

                            await db.Database.ExecuteSqlRawAsync(sql,
                                new SqlParameter("@p0", record.term),
                                new SqlParameter("@p1", name),
                                new SqlParameter("@p2", record.state),
                                new SqlParameter("@p3", record.provenance_uri),
                                new SqlParameter("@p4", record.definition),
                                new SqlParameter("@p5", record.wadename),
                                new SqlParameter("@p6", MapConsumptiveValue(record.category)));
                        }
                        else
                        {
                            var sql = $@"MERGE CVs.{table} AS target
    USING (SELECT @p0 Term, @p1 Name, @p2 State, @p3 Source, @p4 Def, @p5 WaDEName) AS source
        ON target.Name = source.Name
    WHEN MATCHED THEN UPDATE
        SET term = source.term,
            state = source.state,
            sourceVocabularyURI = source.source,
            definition = source.def,
            WaDEName = source.wadename
    WHEN NOT MATCHED THEN 
        INSERT (name, term, state, sourceVocabularyURI, definition, wadename) 
        VALUES (source.name, source.term, source.state, source.source, source.def, source.wadename);";

                            await db.Database.ExecuteSqlRawAsync(sql,
                                new SqlParameter("@p0", record.term),
                                new SqlParameter("@p1", name),
                                new SqlParameter("@p2", record.state),
                                new SqlParameter("@p3", record.provenance_uri),
                                new SqlParameter("@p4", record.definition),
                                new SqlParameter("@p5", record.wadename));
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, $"Failed to update {table} - {name}");
                        throw;
                    }
                }
                ts.Complete();
            }
            log.LogInformation($"Completed Updating CV Table [{table}]");

        }

        private static ConsumptionCategoryType MapConsumptiveValue(string value)
        {
            switch (value)
            {
                case Consumptive:
                    return ConsumptionCategoryType.Consumptive;
                case NonConsumptive:
                    return ConsumptionCategoryType.NonConsumptive;
                default:
                    return ConsumptionCategoryType.Unspecified;
            }
        }
    }
}
