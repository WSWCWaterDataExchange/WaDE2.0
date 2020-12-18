using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                ("regulatoryoverlaytype", "RegulatoryOverlayType")
            };
            await Task.WhenAll(cvData.Select(a => ProcessCvTable(a.Name, a.Table)));
            Console.WriteLine("Done running CvLoader");
            Console.ReadKey();
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
                Console.WriteLine($"Updating table {table}");
                int row = 0;
                try
                {
                    foreach (var record in data)
                    {
                        var name = table == "reportyearcv" ? record.name.Substring(0, 4) : record.name;
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
                        var count = await db.Database.ExecuteSqlCommandAsync(sql,
                            new SqlParameter("@p0", record.term),
                            new SqlParameter("@p1", name),
                            new SqlParameter("@p2", record.state),
                            new SqlParameter("@p3", record.provenance_uri),
                            new SqlParameter("@p4", record.definition),
                            new SqlParameter("@p5", record.wadename));
                        row++;
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error on table {table} in row {row} with message {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
