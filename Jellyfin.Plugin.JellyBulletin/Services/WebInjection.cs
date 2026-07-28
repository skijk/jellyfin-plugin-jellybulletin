using System.Text.RegularExpressions;
using Jellyfin.Plugin.JellyBulletin.Models;

namespace Jellyfin.Plugin.JellyBulletin.Services;

/// <summary>
/// Adds the Bulletin client assets to Jellyfin Web.
/// </summary>
public static partial class WebInjection
{
    public static string PatchIndex(PatchRequestPayload payload)
    {
        var source = payload.Contents ?? string.Empty;
        if (source.Contains("data-jellyfin-bulletin", StringComparison.Ordinal))
        {
            return source;
        }

        const string assets = """
            <link data-jellyfin-bulletin rel="stylesheet" href="Bulletin/Client.css">
            <script data-jellyfin-bulletin defer src="Bulletin/Client.js"></script>
            """;

        return HeadEndRegex().Replace(source, $"{assets}</head>", 1);
    }

    [GeneratedRegex("</head>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex HeadEndRegex();
}
