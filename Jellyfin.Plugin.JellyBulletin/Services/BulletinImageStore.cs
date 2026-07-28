using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.JellyBulletin.Services;

/// <summary>
/// Validates and stores images uploaded through the bulletin editor.
/// </summary>
public sealed class BulletinImageStore
{
    private const long MaxImageBytes = 8 * 1024 * 1024;
    private readonly string _imageDirectory;

    public BulletinImageStore()
    {
        _imageDirectory = Path.Combine(Plugin.Instance.DataFolderPath, "images");
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (file.Length is <= 0 or > MaxImageBytes)
        {
            throw new ArgumentException("Images must be between 1 byte and 8 MB.");
        }

        await using var input = file.OpenReadStream();
        var signature = new byte[12];
        var read = await input.ReadAtLeastAsync(
            signature,
            signature.Length,
            throwOnEndOfStream: false,
            cancellationToken).ConfigureAwait(false);
        var extension = DetectExtension(signature.AsSpan(0, read))
            ?? throw new ArgumentException("Only PNG, JPEG and WebP images are supported.");

        Directory.CreateDirectory(_imageDirectory);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var destination = Path.Combine(_imageDirectory, fileName);
        await using var output = new FileStream(
            destination,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);
        await output.WriteAsync(signature.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
        await input.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
        return fileName;
    }

    public (Stream Stream, string ContentType)? Open(string fileName)
    {
        if (!IsSafeFileName(fileName))
        {
            return null;
        }

        var path = Path.Combine(_imageDirectory, fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        var contentType = Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
        return (File.OpenRead(path), contentType);
    }

    public bool Delete(string fileName)
    {
        if (!IsSafeFileName(fileName))
        {
            return false;
        }

        var path = Path.Combine(_imageDirectory, fileName);
        if (!File.Exists(path))
        {
            return false;
        }

        File.Delete(path);
        return true;
    }

    public void DeleteUnused(IEnumerable<string?> activeImageUrls)
    {
        if (!Directory.Exists(_imageDirectory))
        {
            return;
        }

        const string prefix = "/Bulletin/Image/";
        var activeFiles = activeImageUrls
            .Where(url => url?.StartsWith(prefix, StringComparison.Ordinal) == true)
            .Select(url => url![prefix.Length..])
            .Where(IsSafeFileName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var path in Directory.EnumerateFiles(_imageDirectory))
        {
            var fileName = Path.GetFileName(path);
            if (IsSafeFileName(fileName) && !activeFiles.Contains(fileName))
            {
                File.Delete(path);
            }
        }
    }

    private static string? DetectExtension(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length >= 8
            && bytes[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
        {
            return ".png";
        }

        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
        {
            return ".jpg";
        }

        if (bytes.Length >= 12
            && bytes[..4].SequenceEqual("RIFF"u8)
            && bytes.Slice(8, 4).SequenceEqual("WEBP"u8))
        {
            return ".webp";
        }

        return null;
    }

    private static bool IsSafeFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var stem = Path.GetFileNameWithoutExtension(fileName);
        return Path.GetFileName(fileName) == fileName
            && stem.Length == 32
            && Guid.TryParseExact(stem, "N", out _)
            && extension is ".png" or ".jpg" or ".webp";
    }
}
