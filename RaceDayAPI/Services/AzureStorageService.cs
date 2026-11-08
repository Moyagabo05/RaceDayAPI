using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RaceDayAPI.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        // The configuration is injected automatically by ASP.NET Core
        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _containerName = configuration["AzureBlobStorage:ContainerName"];
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            // 1. Connect to Azure using the connection string from appsettings.json
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // 2. Create the container in Azure if it doesn't exist yet
            await containerClient.CreateIfNotExistsAsync();

            // 3. Create a unique file name so we don't overwrite old files
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{folderName}/{Guid.NewGuid()}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            // 4. Upload the file stream to Azure
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            // 5. Return the public URL so our MVC app can use it
            return blobClient.Uri.ToString();
        }
    }
}