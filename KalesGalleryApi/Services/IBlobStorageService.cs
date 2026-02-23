namespace KalesGalleryApi.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteAsync(string blobName);
    string GetBlobUrl(string blobName);
    Task<IEnumerable<string>> ListBlobUrlsAsync();
}
