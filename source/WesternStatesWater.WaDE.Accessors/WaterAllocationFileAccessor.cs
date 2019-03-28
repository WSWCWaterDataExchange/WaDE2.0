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
            return await GetNormalizedData<AccessorImport.Organization>(runId, "organizations.json");
        }

        async Task<List<AccessorImport.WaterAllocation>> AccessorImport.IWaterAllocationFileAccessor.GetWaterAllocations(string runId)
        {
            return await GetNormalizedData<AccessorImport.WaterAllocation>(runId, "waterallocations.json");
        }

        private async Task<List<T>> GetNormalizedData<T>(string runId, string fileName)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("normalizedimports");
            var blob = cloudBlobContainer.GetBlobReference(Path.Combine(runId, fileName));
            if (!await blob.ExistsAsync())
            {
                return new List<T>();
            }

            using (var ms = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(ms);
                return JsonConvert.DeserializeObject<List<T>>(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
            }
        }
    }

    public class BlobFileAccessor : AccessorImport.IBlobFileAccessor
    {
        public BlobFileAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        async Task<Stream> AccessorImport.IBlobFileAccessor.GetBlobData(string containter, string path)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containter);
            var blob = cloudBlobContainer.GetBlobReference(path);
            if (!await blob.ExistsAsync())
            {
                return new MemoryStream();
            }

            return await blob.OpenReadAsync();
        }

        async Task AccessorImport.IBlobFileAccessor.SaveBlobData(string containter, string path, byte[] data)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containter);

            var blob = cloudBlobContainer.GetBlockBlobReference(path);
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
        }
    }
}
