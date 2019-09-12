using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace CvLoader
{
    class Program
    {
        static async Task Main(string[] args)
        {
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
               // ("naicscode", "naicscode"),
                ("nhdnetworkstatus", "nhdnetworkstatus"),
                ("nhdproduct", "nhdproduct"),
                ("regulatorystatus", "regulatorystatus"),
                ("reportingunittype", "reportingunittype"),
                ("reportyear", "reportyearcv"),
                ("sitetype", "sitetype"),
                ("units", "units"),
               // ("usgscategory", "usgscategory"),
                ("variable", "variable"),
                ("variablespecific", "variablespecific"),
                ("waterallocationbasis", "waterallocationbasis"),
                ("waterqualityindicator", "waterqualityindicator"),
                ("waterallocationtype", "waterallocationtype"),
                ("watersourcetype", "watersourcetype"),
                ("applicableresourcetype", "applicableresourcetype"),
                ("coordinatemethod", "coordinatemethod"),
                
            };
            await Task.WhenAll(cvData.Select(a => ProcessCvTable(a.Name, a.Table)));
        }

        private static async Task ProcessCvTable(string name, string table)
        {
            var data = await FetchData(name);
            var records = ParseData(data);
            await LoadData(table, records);
        }

        private static async Task<string> FetchData(string name)
        {
            return await new WebClient().DownloadStringTaskAsync($"http://vocabulary.westernstateswater.org/api/v1/{name}/?format=csv");
        }

        private static List<dynamic> ParseData(string data)
        {
            using (var reader = new CsvHelper.CsvReader(new StringReader(data)))
            {
                return reader.GetRecords<dynamic>().ToList();
            }
        }

        private static async Task LoadData(string table, List<dynamic> data)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "ConnectionStrings:WadeDatabase", "Server=.;Initial Catalog=WaDE2;Integrated Security=true;" } })
                .Build();

            using (var db = new WaDEContext(config))
            using (var ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }, TransactionScopeAsyncFlowOption.Enabled))
            {
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
        }
    }
}
