using Microsoft.AspNetCore.Components.Forms;

namespace MundialApp.Services;

public sealed class ImageStorageService
{
    public async Task<string> SaveImageAsync(IBrowserFile file, string folderName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.Name);
        var directory = Path.Combine(FileSystem.Current.AppDataDirectory, folderName);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(directory, fileName);

        await using var source = file.OpenReadStream(5 * 1024 * 1024, cancellationToken);
        await using var destination = File.Create(filePath);
        await source.CopyToAsync(destination, cancellationToken);

        return filePath;
    }

    public async Task<string> CreatePreviewAsync(IBrowserFile file, CancellationToken cancellationToken = default)
    {
        await using var source = file.OpenReadStream(5 * 1024 * 1024, cancellationToken);
        using var memoryStream = new MemoryStream();
        await source.CopyToAsync(memoryStream, cancellationToken);
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        return $"data:{file.ContentType};base64,{base64}";
    }

    public async Task<string?> GetImageSourceAsync(string? storedValue, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storedValue))
        {
            return null;
        }

        if (storedValue.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return storedValue;
        }

        var filePath = storedValue;
        if (Uri.TryCreate(storedValue, UriKind.Absolute, out var uri) && uri.IsFile)
        {
            filePath = uri.LocalPath;
        }

        if (!File.Exists(filePath))
        {
            return null;
        }

        var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };

        return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
    }
}
