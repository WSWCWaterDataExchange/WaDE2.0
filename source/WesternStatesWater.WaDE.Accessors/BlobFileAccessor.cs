using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Accessors
{
    public class BlobFileAccessor : AccessorImport.IBlobFileAccessor
    {
        public BlobFileAccessor(IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            Configuration = configuration;
            BlobServiceClient = blobServiceClient;
        }

        private IConfiguration Configuration { get; set; }
        private BlobServiceClient BlobServiceClient { get; set; }

        async Task<Stream> AccessorImport.IBlobFileAccessor.GetBlobData(string container, string path)
        {
            var blobContainer = BlobServiceClient.GetBlobContainerClient(container);
            var blobClient = blobContainer.GetBlobClient(path);

            if (!await blobClient.ExistsAsync())
            {
                return new MemoryStream();
            }

            return await blobClient.OpenReadAsync();
        }

        async Task AccessorImport.IBlobFileAccessor.SaveBlobData(string container, string path, byte[] data)
        {
            var blob = CreateBlobContainer(container, path);

            using (var ms = new MemoryStream(data))
            {
                await blob.UploadAsync(ms);
            }
        }

        async Task AccessorImport.IBlobFileAccessor.SaveBlobData(string container, string path, string data)
        {
            await ((AccessorImport.IBlobFileAccessor)this).SaveBlobData(container, path, System.Text.Encoding.UTF8.GetBytes(data));
        }

        #region Private Methods
        private BlobClient CreateBlobContainer(string container, string path)
        {
            var blobContainer = BlobServiceClient.GetBlobContainerClient(container);
            return blobContainer.GetBlobClient(path);
        }
        #endregion
    }
}
