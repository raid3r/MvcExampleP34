namespace MvcExampleP34.Models.Services;

public class FileStorage(IWebHostEnvironment environment) : IFileStorage
{
    private string GetUploadsPath()
    {
        var uploadsPath = Path.Combine(environment.WebRootPath, "uploads", "images");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }
        return uploadsPath;
    }

    private string GenerateUniqueFileName(string originalFileName)
    {
        var uniqueId = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(originalFileName);
        return $"{uniqueId}{extension}";
    }

    private string GetFilePath(string fileName)
    {
        var dir1 = fileName[0].ToString();
        var dir2 = fileName[1].ToString();
        return Path.Combine(GetUploadsPath(), dir1, dir2, fileName);
    }

    // xy12345-4304-5678-abcd-1234567890ab.jpg
    // av87482-2390-1234-cdef-0987654321ba.png

    // uploads/images/x/y/xy12345-4304-5678-abcd-1234567890ab.jpg
    // uploads/images/a/v/av87482-2390-1234-cdef-0987654321ba.png


    public async Task<string> SaveFileAsync(IFormFile formFile, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = GenerateUniqueFileName(formFile.FileName);
        var filePath = GetFilePath(uniqueFileName);

        var dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir!);
        }

        using var stream = new FileStream(filePath, FileMode.Create);
        await formFile.CopyToAsync(stream, cancellationToken);

        return uniqueFileName;
    }

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}
