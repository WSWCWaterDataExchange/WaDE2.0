using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("normalizedimports");
            var blob = cloudBlobContainer.GetBlobReference(Path.Combine(runId, "organizations.json"));
            if(!await blob.ExistsAsync())
            {
                return new List<AccessorImport.Organization>();
            }

            using (var ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);
                return JsonConvert.DeserializeObject<List<AccessorImport.Organization>>(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
            }
        }
    }
}
