using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;

namespace WesternStatesWater.WaDE.Managers.Import
{
    public class FlattenManager : ManagerImport.IFlattenManager
    {
        public FlattenManager(IBlobFileAccessor blobFileAccessor)
        {
            BlobFileAccessor = blobFileAccessor;
        }

        public IBlobFileAccessor BlobFileAccessor { get; set; }

        async Task ManagerImport.IFlattenManager.Flatten(string container, string folder, string sourceFileName, string destFileName, string keyCol, string valueCol)
        {
            //get the source file from blob storage
            var rawData = await BlobFileAccessor
                .GetBlobData(container, Path.Combine(folder, sourceFileName));


            //ignore bad data and missing fields; we don't care
            var csvConfig = new Configuration
            {
                BadDataFound = null,
                MissingFieldFound = null
            };

            var rawRecords = new List<FlattenedRaw>();

            using (var reader = new StreamReader(rawData))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var flattenedRawMap = new FlattenedRawMap(keyCol, valueCol);

                csv.Configuration.RegisterClassMap(flattenedRawMap);

                rawRecords = csv.GetRecords<FlattenedRaw>().ToList();
            }

            //flatten the data into <key>, <csv values> pairs
            var flattenedKeys = new List<FlattenedKey>();

            //TODO: bottleneck!
            foreach (var record in rawRecords)
            {
                if (!string.IsNullOrEmpty(record.Key) &&
                    !string.IsNullOrEmpty(record.Value))
                {
                    if (!flattenedKeys.Select(x => x.Text).Contains(record.Key))
                    {
                        //new key
                        flattenedKeys.Add(new FlattenedKey
                        {
                            Text = record.Key,
                            FlattenedValues = new List<FlattenedValue> { new FlattenedValue { Text = record.Value } }
                        });
                    }
                    else
                    {
                        //existing key
                        var flattenedKey = flattenedKeys.Single(x => x.Text.Equals(record.Key));

                        if (!flattenedKey.FlattenedValues.Select(x => x.Text).Contains(record.Value))
                        {
                            flattenedKey.FlattenedValues.Add(new FlattenedValue { Text = record.Value });
                        }
                    }
                }
            }

            //write the result back to blob storage
            var flattenedRecords = new List<object>();

            foreach (var flattenedKey in flattenedKeys)
            {
                flattenedRecords.Add(new { Id = flattenedKey.Text, Value = flattenedKey.CsvValues });
            }

            var csvData = string.Empty;

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecords(flattenedRecords);
                writer.Flush();

                stream.Position = 0;

                csvData = reader.ReadToEnd();
            }

            await BlobFileAccessor.SaveBlobData(container, Path.Combine(folder, destFileName), csvData);
        }
    }
}
