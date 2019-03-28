using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Accessors
{
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
