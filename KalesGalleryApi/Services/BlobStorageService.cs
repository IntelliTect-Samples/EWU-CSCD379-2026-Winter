using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace KalesGalleryApi.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        var containerName = configuration["AzureBlob:ContainerName"] ?? "artgallery";
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        // Ensure container exists
        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = _containerClient.GetBlobClient(fileName);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = contentType
        };

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeaders
        });

        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.DeleteIfExistsAsync();
        return response.Value;
    }

    public string GetBlobUrl(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }

    public async Task<IEnumerable<string>> ListBlobUrlsAsync()
    {
        var urls = new List<string>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync())
        {
            urls.Add(GetBlobUrl(blobItem.Name));
        }

        return urls;
    }
}