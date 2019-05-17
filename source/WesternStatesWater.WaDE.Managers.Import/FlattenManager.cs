using CsvHelper;
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

            //flatten the data into <key>, <csv values> pairs
            var flattenedKeys = new List<FlattenedKey>();

            using (var reader = new StreamReader(rawData))
            using (var csv = new CsvReader(reader))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var keyVal = csv.GetField<string>(keyCol);
                    var valVal = csv.GetField<string>(valueCol);

                    if (!flattenedKeys.Select(x => x.Text).Contains(keyVal))
                    {
                        //new key
                        flattenedKeys.Add(new FlattenedKey
                        {
                            Text = keyVal,
                            FlattenedValues = new List<FlattenedValue> { new FlattenedValue { Text = valVal } }
                        });
                    }
                    else
                    {
                        //existing key
                        var flattenedKey = flattenedKeys.Single(x => x.Text.Equals(keyVal));

                        flattenedKey.FlattenedValues.Add(new FlattenedValue { Text = valVal });
                    }
                }
            }

            //write the result back to blob storage
            var records = new List<object>();

            foreach (var flattenedKey in flattenedKeys)
            {
                records.Add(new { Id = flattenedKey.Text, Value = flattenedKey.CsvValues });
            }

            var csvData = string.Empty;

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(records);
                writer.Flush();

                stream.Position = 0;

                csvData = reader.ReadToEnd();
            }

            await BlobFileAccessor.SaveBlobData(container, Path.Combine(folder, destFileName), csvData);
        }
    }
}
