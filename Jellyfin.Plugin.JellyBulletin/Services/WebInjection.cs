using System.Text.RegularExpressions;
using System.Text;

namespace Jellyfin.Plugin.JellyBulletin.Services;

/// <summary>
/// Adds the Bulletin client assets to Jellyfin Web.
/// </summary>
public static partial class WebInjection
{
    public static async Task TransformIndex(string path, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            leaveOpen: true);
        var source = await reader.ReadToEndAsync().ConfigureAwait(false);

        if (source.Contains("data-jellyfin-bulletin", StringComparison.Ordinal))
        {
            stream.Seek(0, SeekOrigin.Begin);
            return;
        }

        const string assets = """
            <link data-jellyfin-bulletin rel="stylesheet" href="/Bulletin/Client.css">
            <script data-jellyfin-bulletin defer src="/Bulletin/Client.js"></script>
            """;

        var transformed = HeadEndRegex().Replace(source, $"{assets}</head>", 1);
        var bytes = Encoding.UTF8.GetBytes(transformed);
        stream.SetLength(0);
        stream.Seek(0, SeekOrigin.Begin);
        await stream.WriteAsync(bytes).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);
    }

    [GeneratedRegex("</head>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex HeadEndRegex();
}
