using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Accessors
{
    public class DataIngestionFileAccessor : AccessorImport.IDataIngestionFileAccessor
    {
        public DataIngestionFileAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        async Task<List<AccessorImport.Organization>> AccessorImport.IDataIngestionFileAccessor.GetOrganizations(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.Organization>(runId, "organizations.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetOrganizationsCount(string runId)
        {
            return await GetRecordCount(runId, "organizations.csv");
        }

        async Task<List<AccessorImport.WaterAllocation>> AccessorImport.IDataIngestionFileAccessor.GetWaterAllocations(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.WaterAllocation>(runId, "waterallocations.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetWaterAllocationsCount(string runId)
        {
            return await GetRecordCount(runId, "waterallocations.csv");
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IDataIngestionFileAccessor.GetAggregatedAmounts(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.AggregatedAmount>(runId, "aggregatedamounts.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetAggregatedAmountsCount(string runId)
        {
            return await GetRecordCount(runId, "aggregatedamounts.csv");
        }

        async Task<List<AccessorImport.Method>> AccessorImport.IDataIngestionFileAccessor.GetMethods(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.Method>(runId, "methods.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetMethodsCount(string runId)
        {
            return await GetRecordCount(runId, "methods.csv");
        }

        async Task<List<AccessorImport.RegulatoryOverlay>> AccessorImport.IDataIngestionFileAccessor.GetRegulatoryOverlays(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.RegulatoryOverlay>(runId, "regulatoryoverlays.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetRegulatoryOverlaysCount(string runId)
        {
            return await GetRecordCount(runId, "regulatoryoverlays.csv");
        }

        async Task<List<AccessorImport.ReportingUnit>> AccessorImport.IDataIngestionFileAccessor.GetReportingUnits(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.ReportingUnit>(runId, "reportingunits.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetReportingUnitsCount(string runId)
        {
            return await GetRecordCount(runId, "reportingunits.csv");
        }

        async Task<List<AccessorImport.RegulatoryReportingUnits>> AccessorImport.IDataIngestionFileAccessor.GetRegulatoryReportingUnits(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.RegulatoryReportingUnits>(runId, "regulatoryreportingunits.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetRegulatoryReportingUnitsCount(string runId)
        {
            return await GetRecordCount(runId, "regulatoryreportingunits.csv");
        }

        async Task<List<AccessorImport.Site>> AccessorImport.IDataIngestionFileAccessor.GetSites(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.Site>(runId, "sites.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetSitesCount(string runId)
        {
            return await GetRecordCount(runId, "sites.csv");
        }

        async Task<List<AccessorImport.SiteSpecificAmount>> AccessorImport.IDataIngestionFileAccessor.GetSiteSpecificAmounts(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.SiteSpecificAmount>(runId, "sitespecificamounts.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetSiteSpecificAmountsCount(string runId)
        {
            return await GetRecordCount(runId, "sitespecificamounts.csv");
        }

        async Task<List<AccessorImport.Variable>> AccessorImport.IDataIngestionFileAccessor.GetVariables(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.Variable>(runId, "variables.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetVariablesCount(string runId)
        {
            return await GetRecordCount(runId, "variables.csv");
        }

        async Task<List<AccessorImport.WaterSource>> AccessorImport.IDataIngestionFileAccessor.GetWaterSources(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.WaterSource>(runId, "watersources.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetWaterSourcesCount(string runId)
        {
            return await GetRecordCount(runId, "watersources.csv");
        }

        async Task<List<AccessorImport.PODSitePOUSite>> AccessorImport.IDataIngestionFileAccessor.GetPODSiteToPOUSiteRelationships(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.PODSitePOUSite>(runId, "podsitetopousiterelationships.csv", startIndex, count);
        }

        async Task<int> AccessorImport.IDataIngestionFileAccessor.GetPODSiteToPOUSiteRelationshipsCount(string runId)
        {
            return await GetRecordCount(runId, "podsitetopousiterelationships.csv");
        }

        private async Task<List<T>> GetNormalizedData<T>(string runId, string fileName, int startIndex, int count)
        {
            var stream = await FileStreamFactory.GetStreamAsync(Configuration, runId, fileName);
            if (stream == null)
            {
                return new List<T>();
            }
            using (stream)
            {
                bool isRecordBad = false;
                var csvConfig = new Configuration
                {
                    Delimiter = ",",
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true,
                    IgnoreQuotes = false,
                    BadDataFound = (a) =>
                    {
                        isRecordBad = true;
                    }
                };

                csvConfig.TypeConverterCache.AddConverter<DateTime?>(new DMYDateConverter());

                var currIndex = 0;
                var results = new List<T>();

                using (var reader = new CsvReader(new CsvParser(new StreamReader(stream), csvConfig)))
                {
                    while (reader.Read())
                    {
                        if (!isRecordBad)
                        {
                            if (currIndex >= startIndex + count)
                            {
                                break;
                            }
                            if (currIndex >= startIndex)
                            {
                                results.Add(reader.GetRecord<T>());
                            }
                            else if (currIndex == 0)
                            {
                                reader.GetRecord<T>(); //If we don't read the first record, CSVHelper errors
                            }
                            currIndex++;
                        }
                        isRecordBad = false;
                    }
                }
                return results;
            }
        }

        private async Task<int> GetRecordCount(string runId, string fileName)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference("normalizedimports");

            var blob = cloudBlobContainer.GetBlockBlobReference(Path.Combine(runId, fileName));

            if (!await blob.ExistsAsync())
            {
                return 0;
            }

            bool isRecordBad = false;
            var csvConfig = new Configuration
            {
                Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                IgnoreQuotes = false,
                BadDataFound = (a) =>
                {
                    isRecordBad = true;
                }
            };

            csvConfig.TypeConverterCache.AddConverter<DateTime?>(new DMYDateConverter());

            using (var stream = await blob.OpenReadAsync())
            {
                var count = -1; //account for the header
                using (var reader = new CsvReader(new CsvParser(new StreamReader(stream), csvConfig)))
                {
                    while (reader.Read())
                    {
                        if (!isRecordBad)
                        {
                            count++;
                        }

                        isRecordBad = false;
                    }
                }
                return count == -1 ? 0 : count;
            }
        }

        public class DMYDateConverter : CsvHelper.TypeConversion.DateTimeConverter
        {
            private const string dateFormat1 = @"yyyy-M-d";
            private const string dateFormat2 = @"yyyy-MM-dd";

            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                DateTime? newDate = null;

                if (DateTime.TryParseExact(text, dateFormat1, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parseDate1))
                {
                    newDate = parseDate1;
                }
                else if (DateTime.TryParseExact(text, dateFormat2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parseDate2))
                {
                    newDate = parseDate2;
                }
                else if (DateTime.TryParse(text, out var parseDate3))
                {
                    newDate = parseDate3;
                }

                return newDate;
            }
        }

        private class FileStreamFactoryImpl : IFileStreamFactory
        {
            public async Task<Stream> GetStreamAsync(IConfiguration configuration, string runId, string fileName)
            {
                var storageConnectionString = configuration.GetConnectionString("AzureStorage");
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = blobClient.GetContainerReference("normalizedimports");

                var blob = cloudBlobContainer.GetBlockBlobReference(Path.Combine(runId, fileName));

                if (!await blob.ExistsAsync())
                {
                    return null;
                }

                return await blob.OpenReadAsync();
            }
        }

        private IFileStreamFactory _fileStreamFactory;
        internal IFileStreamFactory FileStreamFactory
        {
            get => _fileStreamFactory ?? (_fileStreamFactory = new FileStreamFactoryImpl());
            set => _fileStreamFactory = value;
        }
    }

    internal interface IFileStreamFactory
    {
        Task<Stream> GetStreamAsync(IConfiguration configuration, string runId, string fileName);
    }


}
