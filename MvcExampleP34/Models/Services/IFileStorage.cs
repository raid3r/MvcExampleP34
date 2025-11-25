namespace MvcExampleP34.Models.Services;

public interface IFileStorage
{
    public Task<string> SaveFileAsync(IFormFile formFile, CancellationToken cancellationToken = default);

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
}
