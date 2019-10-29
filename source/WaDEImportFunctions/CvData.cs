using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WaDEImportFunctions
{
    public class CvData
    {
        public CvData(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        [FunctionName("CvDataUpdate")]
        public async Task Update([QueueTrigger("cv-data-update", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
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

            };
            await Task.WhenAll(cvData.Select(a => ProcessCvTable(a.Name, a.Table, log)));

            log.LogInformation($"Completed Updating All CV Data");
        }

        private async Task ProcessCvTable(string name, string table, ILogger log)
        {
            var data = await FetchData(name);
            var records = ParseData(data);
            await LoadData(table, records, log);
        }

        private async Task<string> FetchData(string name)
        {
            return await new WebClient().DownloadStringTaskAsync($"http://vocabulary.westernstateswater.org/api/v1/{name}/?format=csv");
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
                    var sql = $@"MERGE CVs.{table} AS target
    USING (SELECT '{record.term}' Term, '{name}' Name, '{record.state}' State, '{record.provenance_uri}' Source, '{record.definition}' Def) AS source
        ON target.Name = source.Name
    WHEN MATCHED THEN UPDATE
        SET term = source.term,
            state = source.state,
            sourceVocabularyURI = source.source,
            definition = source.def
    WHEN NOT MATCHED THEN 
        INSERT (name, term, state, sourceVocabularyURI, definition) 
        VALUES (source.name, source.term, source.state, source.source, source.def);";
                    await db.Database.ExecuteSqlCommandAsync(sql);

                }
                ts.Complete();
            }
            log.LogInformation($"Completed Updating CV Table [{table}]");
        }
    }
}