using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Accessors
{
    public class WaterAllocationFileAccessor : AccessorImport.IWaterAllocationFileAccessor
    {
        public WaterAllocationFileAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        async Task<List<AccessorImport.Organization>> AccessorImport.IWaterAllocationFileAccessor.GetOrganizations(string runId, int startIndex, int count)
        {
            return await GetNormalizedData<AccessorImport.Organization>(runId, "organizations.csv", startIndex, count);
        }

        async Task<List<AccessorImport.WaterAllocation>> AccessorImport.IWaterAllocationFileAccessor.GetWaterAllocations(string runId)
        {
            return await GetNormalizedData<AccessorImport.WaterAllocation>(runId, "waterallocations.csv");
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetAggregatedAmounts(string runId)
        {
            return await GetNormalizedData<AccessorImport.AggregatedAmount>(runId, "aggregatedamounts.csv");
        }

        async Task<List<AccessorImport.Method>> AccessorImport.IWaterAllocationFileAccessor.GetMethods(string runId)
        {
            return await GetNormalizedData<AccessorImport.Method>(runId, "methods.csv");
        }

        async Task<List<AccessorImport.RegulatoryOverlay>> AccessorImport.IWaterAllocationFileAccessor.GetRegulatoryOverlays(string runId)
        {
            return await GetNormalizedData<AccessorImport.RegulatoryOverlay>(runId, "regulatoryoverlays.csv");
        }

        async Task<List<AccessorImport.ReportingUnit>> AccessorImport.IWaterAllocationFileAccessor.GetReportingUnits(string runId)
        {
            return await GetNormalizedData<AccessorImport.ReportingUnit>(runId, "reportingunits.csv");
        }

        async Task<List<AccessorImport.RegulatoryReportingUnits>> AccessorImport.IWaterAllocationFileAccessor.GetRegulatoryReportingUnits(string runId)
        {
            return await GetNormalizedData<AccessorImport.RegulatoryReportingUnits>(runId, "regulatoryreportingunits.csv");
        }

       

        async Task<List<AccessorImport.Site>> AccessorImport.IWaterAllocationFileAccessor.GetSites(string runId)
        {
            return await GetNormalizedData<AccessorImport.Site>(runId, "sites.csv");
        }

        async Task<List<AccessorImport.SiteSpecificAmount>> AccessorImport.IWaterAllocationFileAccessor.GetSiteSpecificAmounts(string runId)
        {
            return await GetNormalizedData<AccessorImport.SiteSpecificAmount>(runId, "sitespecificamounts.csv");
        }

        async Task<List<AccessorImport.Variable>> AccessorImport.IWaterAllocationFileAccessor.GetVariables(string runId)
        {
            return await GetNormalizedData<AccessorImport.Variable>(runId, "variables.csv");
        }

        async Task<List<AccessorImport.WaterSource>> AccessorImport.IWaterAllocationFileAccessor.GetWaterSources(string runId)
        {
            return await GetNormalizedData<AccessorImport.WaterSource>(runId, "watersources.csv");
        }

        private async Task<List<T>> GetNormalizedData<T>(string runId, string fileName, int startIndex = 0, int count = int.MaxValue)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference("normalizedimports");

            var blob = cloudBlobContainer.GetBlockBlobReference(Path.Combine(runId, fileName));

            if (!await blob.ExistsAsync())
            {
                return new List<T>();
            }

            var csvConfig = new Configuration
            {
                Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                IgnoreQuotes = false
            };

            csvConfig.TypeConverterCache.AddConverter<DateTime?>(new DMYDateConverter());

            var stream = await blob.OpenReadAsync();
            var skippedCount = 0;
            using (var reader = new CsvReader(new CsvParser(new StreamReader(stream), csvConfig)))
            {
                while(skippedCount < startIndex)
                {
                    reader
                }
                return reader.GetRecords<T>().Skip(startIndex).Take(count).ToList();
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

            var csvConfig = new Configuration
            {
                Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                IgnoreQuotes = false
            };

            csvConfig.TypeConverterCache.AddConverter<DateTime?>(new DMYDateConverter());

            var stream = await blob.OpenReadAsync();

            var count = 0;
            using (var reader = new CsvReader(new CsvParser(new StreamReader(stream), csvConfig)))
            {
                while (reader.Read())
                {
                    count++;
                }
            }
            return count;
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
    }
}
