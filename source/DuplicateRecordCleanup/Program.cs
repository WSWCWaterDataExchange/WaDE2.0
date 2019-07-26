using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Linq;

namespace DuplicateRecordCleanup
{
    class Program
    {
        static void Main(string[] args)
        {
            //assuming the unique key is in the first column
            var config = new CsvHelper.Configuration.Configuration
            {
                HasHeaderRecord = false,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim,
                IgnoreQuotes = true
            };
            config.RegisterClassMap<RecordDataLayoutMap>();
            using (var fileReader = System.IO.File.OpenText(@"C:\temp\WaDEImportFiles\sites.csv"))
            using (var csvReader = new CsvHelper.CsvReader(fileReader, config))
            {
                var allRecords = csvReader.GetRecords<RecordData>();
                System.IO.File.WriteAllLines(@"C:\temp\WaDEImportFiles\sites_deduped.csv", allRecords.GroupBy(a => a.Key.ToLower())
                    .Select(a => a.First())
                    .OrderBy(a=>a.RecordNumber)
                    .Select(a=>a.Record));
            }
        }

        private class RecordData
        {
            public string Key { get; set; }
            public string Record { get; set; }
            public int RecordNumber { get; set; }
        }

        private class RecordDataLayoutMap : ClassMap<RecordData>
        {
            public RecordDataLayoutMap()
            {
                Map(m => m.Key).Index(0);
                Map(m => m.RecordNumber).ConvertUsing(GetRecordNumber);
                Map(m => m.Record).ConvertUsing(GetRecord);
            }

            private string GetRecord(IReaderRow row)
            {
                return row.Context.RawRecord.TrimEnd('\r', '\n');
            }

            private int GetRecordNumber(IReaderRow row)
            {
                return row.Context.Row;
            }
        }
    }
}
