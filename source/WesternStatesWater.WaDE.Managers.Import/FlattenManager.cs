using CsvHelper;
using CsvHelper.Configuration;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
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

        async Task ManagerImport.IFlattenManager.CoordinateProjection(string container, string folder, string sourceFileName, string destFileName, string xValueCol, string yValueCol)
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

            var rawRecords = new List<CoordinateRaw>();

            using (var reader = new StreamReader(rawData))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                //remap the targeted csv columns into generic Key/Value fields
                var coordinateRawMap = new CoordinateRawMap(xValueCol, yValueCol);

                csv.Configuration.RegisterClassMap(coordinateRawMap);

                rawRecords = csv.GetRecords<CoordinateRaw>().ToList();
            }

            //project UTM Zone 12 coordinates into WGS84
            var ctfac = new CoordinateTransformationFactory();

            //assumption: this is Utah, and they're in zone 12
            var utm = ProjectedCoordinateSystem.WGS84_UTM(12, true);

            var trans = ctfac.CreateFromCoordinateSystems(utm, GeographicCoordinateSystem.WGS84);

            //if they don't have convertible coordinates, we don't care
            //then, group by the coordinate pairs and get just the first of each group, so we have distinct sets of coordinates
            rawRecords = rawRecords
                .Where(x => !string.IsNullOrWhiteSpace(x.XCoord) &&
                    !string.IsNullOrWhiteSpace(x.YCoord) &&
                    double.TryParse(x.XCoord, out double xTemp) &&
                    double.TryParse(x.YCoord, out double yTemp))
                .GroupBy(x => new { x.XCoord, x.YCoord })
                .Select(x => x.First())
                .ToList();

            //new converted coordinates
            var coordinateResults = new List<CoordinateResult>();

            foreach (var record in rawRecords)
            {
                var xCoord = double.Parse(record.XCoord);
                var yCoord = double.Parse(record.YCoord);

                var result = trans.MathTransform.Transform(new double[] { xCoord, yCoord });

                coordinateResults.Add(new CoordinateResult
                {
                    XCoord = record.XCoord,
                    YCoord = record.YCoord,
                    Latitude = result[1].ToString(),
                    Longitude = result[0].ToString()
                });
            }

            //convert to object array for CsvWriter
            var coordRecords = coordinateResults
                .Select(x => new
                {
                    x.XCoord,
                    x.YCoord,
                    x.Latitude,
                    x.Longitude
                })
                .ToList();

            var csvData = string.Empty;

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecords(coordRecords);
                writer.Flush();

                stream.Position = 0;

                csvData = reader.ReadToEnd();
            }

            await BlobFileAccessor.SaveBlobData(container, Path.Combine(folder, destFileName), csvData);
        }

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
                ///////////////////////////////////////////////////
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                ///////////////////////////////////////////////////////

                rawRecords = csv.GetRecords<FlattenedRaw>().ToList();
            }

            var flattenedRecords = rawRecords
                .GroupBy(x => x.Key)
                .Select(x => new
                {
                    Id = x.Key,
                    Value = string.Join(",", x.Select(y => y.Value).Distinct())
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
