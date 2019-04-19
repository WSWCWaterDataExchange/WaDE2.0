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

        async Task<List<AccessorImport.Organization>> AccessorImport.IWaterAllocationFileAccessor.GetOrganizations(string runId)
        {
            return await GetNormalizedData<AccessorImport.Organization>(runId, "organizations.csv");
        }

        async Task<List<AccessorImport.WaterAllocation>> AccessorImport.IWaterAllocationFileAccessor.GetWaterAllocations(string runId)
        {
            return await GetNormalizedData<AccessorImport.WaterAllocation>(runId, "waterallocations.csv");
        }

        private async Task<List<T>> GetNormalizedData<T>(string runId, string fileName, ClassMap<T> classMap = null)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("normalizedimports");
            var blob = cloudBlobContainer.GetBlockBlobReference(Path.Combine(runId, fileName));
            if (!await blob.ExistsAsync())
            {
                return new List<T>();
            }

            var csvConfig = new Configuration();
            csvConfig.Delimiter = ",";
            csvConfig.TrimOptions = TrimOptions.Trim;
            csvConfig.IgnoreBlankLines = true;
            csvConfig.IgnoreQuotes = false;
            csvConfig.TypeConverterCache.AddConverter<DateTime?>(new DMYDateConverter());
            if(classMap != null)
            {
                csvConfig.RegisterClassMap(classMap);
            }

            var text = (await blob.DownloadTextAsync()).TrimStart(new char[] { '\uFEFF', '\u200B' });

            var result = new List<T>();
            using (var reader = new CsvReader(new CsvParser(new StringReader(text), csvConfig)))
            {
                return reader.GetRecords<T>().ToList();
            }
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetAggregations(string runId)
        {
            throw new NotImplementedException();
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetMethods(string runId)
        {
            throw new NotImplementedException();
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetRegulatoryOverlays(string runId)
        {
            throw new NotImplementedException();
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetReportingUnits(string runId)
        {
            throw new NotImplementedException();
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetSites(string runId)
        {
            throw new NotImplementedException();
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetSiteSpecificAmounts(string runId)
        {
            throw new NotImplementedException();
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetVariables(string runId)
        {
            throw new NotImplementedException();
        }

        async Task<List<AccessorImport.AggregatedAmount>> AccessorImport.IWaterAllocationFileAccessor.GetWaterSources(string runId)
        {
            throw new NotImplementedException();
        }

        public class DMYDateConverter : CsvHelper.TypeConversion.DateTimeConverter
        {
            private const string dateFormat1 = @"yyyy-M-d";
            private const string dateFormat2 = @"yyyy-MM-dd";

            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                DateTime? newDate = default(System.DateTime?);
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
