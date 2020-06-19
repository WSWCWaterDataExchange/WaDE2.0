using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
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

        async Task<Stream> AccessorImport.IBlobFileAccessor.GetBlobData(string container, string path)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference(container);

            var blob = cloudBlobContainer.GetBlobReference(path);
            if (!await blob.ExistsAsync())
            {
                return new MemoryStream();
            }

            return await blob.OpenReadAsync();
        }

        async Task AccessorImport.IBlobFileAccessor.SaveBlobData(string container, string path, byte[] data)
        {
            var blob = CreateBlobContainer(container, path);

            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
        }

        async Task AccessorImport.IBlobFileAccessor.SaveBlobData(string container, string path, string data)
        {
            var blob = CreateBlobContainer(container, path);

            await blob.UploadTextAsync(data);
        }

        #region Private Methods
        private CloudBlockBlob CreateBlobContainer(string container, string path)
        {
            var storageConnectionString = Configuration.GetConnectionString("AzureStorage");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference(container);
            var blob = cloudBlobContainer.GetBlockBlobReference(path);

            return blob;
        }
        #endregion
    }
}
