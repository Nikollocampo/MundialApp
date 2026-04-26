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

        return new Uri(filePath).AbsoluteUri;
    }
}
