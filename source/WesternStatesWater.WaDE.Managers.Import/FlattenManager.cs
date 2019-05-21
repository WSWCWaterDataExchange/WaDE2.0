using CsvHelper;
using CsvHelper.Configuration;
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
                //remap the targeted csv columns into generic Key/Value fields
                var flattenedRawMap = new FlattenedRawMap(keyCol, valueCol);

                csv.Configuration.RegisterClassMap(flattenedRawMap);

                rawRecords = csv.GetRecords<FlattenedRaw>().ToList();
            }

            //distinct set of keys
            var distinctKeys = rawRecords
                .Select(x => x.Key)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList();

            //new flattened data structure
            var flattenedKeys = new List<FlattenedKey>();

            //parallel processing for performance!
            Parallel.ForEach(distinctKeys, key =>
            {
                //get the values to flatten for the given key
                var flattenedValues = rawRecords
                    .Where(x => x.Key.Equals(key))
                    .Select(x => x.Value)
                    .Where(y => !string.IsNullOrEmpty(y))
                    .Distinct()
                    .Select(z => new FlattenedValue
                    {
                        Text = z
                    })
                    .ToList();

                //add it to the collection
                var flattenedKey = new FlattenedKey
                {
                    Text = key,
                    FlattenedValues = flattenedValues
                };

                flattenedKeys.Add(flattenedKey);
            });

            //convert to object array of Id/Value pairs for CsvWriter
            var flattenedRecords = flattenedKeys
                .Select(x => new
                {
                    Id = x.Text,
                    Value = x.CsvValues
                })
                .ToList();

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
