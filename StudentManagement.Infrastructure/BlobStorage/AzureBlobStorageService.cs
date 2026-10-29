using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Infrastructure.BlobStorage
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is not configured.");

            var containerName =
                configuration["AzureStorage:ContainerName"]
                ?? throw new InvalidOperationException(
                    "Azure Storage container name is not configured.");

            var blobServiceClient =
                new BlobServiceClient(connectionString);

            _containerClient =
                blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> UploadAsync(
            Stream fileStream,
            string blobName,
            string contentType)
        {
            await _containerClient.CreateIfNotExistsAsync();

            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                });

            return blobName;
        }

        public async Task DeleteAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
