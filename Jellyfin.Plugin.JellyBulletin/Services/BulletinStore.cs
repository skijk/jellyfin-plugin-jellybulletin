using System.Text.RegularExpressions;
using Jellyfin.Plugin.JellyBulletin.Models;
using Newtonsoft.Json;

namespace Jellyfin.Plugin.JellyBulletin.Services;

/// <summary>
/// Reads, validates and persists bulletin content.
/// </summary>
public sealed partial class BulletinStore
{
    private const int MaxItems = 250;
    private const int MaxTitleLength = 160;
    private const int MaxAltTextLength = 300;
    private const int MaxTextLength = 10000;
    private readonly BulletinImageStore _images;

    public BulletinStore(BulletinImageStore images)
    {
        _images = images;
    }

    public BulletinResponse GetPublished()
    {
        var configuration = Plugin.Instance.Configuration;
        var now = DateTimeOffset.UtcNow;
        var items = ReadItems()
            .Where(item => item.IsPublished
                && (!item.PublishAt.HasValue || item.PublishAt.Value <= now)
                && (!item.UnpublishAt.HasValue || item.UnpublishAt.Value > now))
            .OrderByDescending(item => item.IsPinned)
            .ThenBy(item => item.SortOrder)
            .ThenByDescending(item => item.PublishedAt)
            .Take(Math.Clamp(configuration.VisibleItemCount, 3, 5))
            .ToList();

        return new BulletinResponse
        {
            VisibleItemCount = Math.Clamp(configuration.VisibleItemCount, 3, 5),
            PanelHeight = NormalizePanelHeight(configuration.PanelHeight),
            ShowImages = configuration.ShowImages,
            AutoRotate = configuration.AutoRotate,
            RotationIntervalSeconds = Math.Clamp(configuration.RotationIntervalSeconds, 5, 30),
            Items = items
        };
    }

    public SaveBulletinsRequest GetAll()
    {
        return new SaveBulletinsRequest
        {
            VisibleItemCount = Math.Clamp(Plugin.Instance.Configuration.VisibleItemCount, 3, 5),
            PanelHeight = NormalizePanelHeight(Plugin.Instance.Configuration.PanelHeight),
            ShowImages = Plugin.Instance.Configuration.ShowImages,
            AutoRotate = Plugin.Instance.Configuration.AutoRotate,
            RotationIntervalSeconds = Math.Clamp(Plugin.Instance.Configuration.RotationIntervalSeconds, 5, 30),
            Items = ReadItems()
                .OrderByDescending(item => item.IsPinned)
                .ThenBy(item => item.SortOrder)
                .ThenByDescending(item => item.PublishedAt)
                .ToList()
        };
    }

    public void Save(SaveBulletinsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Items.Count > MaxItems)
        {
            throw new ArgumentException($"At most {MaxItems} news items may be stored.");
        }

        var now = DateTimeOffset.UtcNow;
        for (var index = 0; index < request.Items.Count; index++)
        {
            var item = request.Items[index];
            ValidateItem(item);
            item.Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;
            item.UpdatedAt = now;
            item.PublishedAt = item.PublishedAt == default ? now : item.PublishedAt;
            item.IsPublished = item.IsPublished || item.PublishAt.HasValue;
            item.SortOrder = index;
        }

        var configuration = Plugin.Instance.Configuration;
        configuration.VisibleItemCount = Math.Clamp(request.VisibleItemCount, 3, 5);
        configuration.PanelHeight = NormalizePanelHeight(request.PanelHeight);
        configuration.ShowImages = request.ShowImages;
        configuration.AutoRotate = request.AutoRotate;
        configuration.RotationIntervalSeconds = Math.Clamp(request.RotationIntervalSeconds, 5, 30);
        configuration.NewsJson = JsonConvert.SerializeObject(request.Items);
        Plugin.Instance.UpdateConfiguration(configuration);
        _images.DeleteUnused(request.Items.Select(item => item.ImageUrl));
    }

    private static void ValidateItem(BulletinItem item)
    {
        item.Title = (item.Title ?? string.Empty).Trim();
        if (item.Title.Length is 0 or > MaxTitleLength)
        {
            throw new ArgumentException($"Titles must contain 1-{MaxTitleLength} characters.");
        }

        item.ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim();
        if (item.ImageUrl is not null
            && !IsLocalImageUrl(item.ImageUrl)
            && (!Uri.TryCreate(item.ImageUrl, UriKind.Absolute, out var imageUri)
                || (imageUri.Scheme != Uri.UriSchemeHttp && imageUri.Scheme != Uri.UriSchemeHttps)))
        {
            throw new ArgumentException("Image URLs must use HTTP, HTTPS, or a JellyBulletin upload.");
        }

        item.ImageAlt = string.IsNullOrWhiteSpace(item.ImageAlt) ? null : item.ImageAlt.Trim();
        if (item.ImageAlt?.Length > MaxAltTextLength)
        {
            throw new ArgumentException($"Image alternative text must not exceed {MaxAltTextLength} characters.");
        }

        if (item.PublishAt.HasValue
            && item.UnpublishAt.HasValue
            && item.UnpublishAt.Value <= item.PublishAt.Value)
        {
            throw new ArgumentException("The unpublish time must be later than the publish time.");
        }

        foreach (var block in item.Blocks)
        {
            if (block.Type is not ("paragraph" or "bulletList" or "numberedList"))
            {
                throw new ArgumentException("Unsupported content block.");
            }

            ValidateInlines(block.Content);
            foreach (var listItem in block.Items)
            {
                ValidateInlines(listItem);
            }
        }
    }

    private static void ValidateInlines(IEnumerable<BulletinInline> inlines)
    {
        foreach (var inline in inlines)
        {
            inline.Text ??= string.Empty;
            if (inline.Text.Length > MaxTextLength)
            {
                throw new ArgumentException("A text section is too long.");
            }

            if (inline.Color is not null && !HexColorRegex().IsMatch(inline.Color))
            {
                throw new ArgumentException("Unsupported text color.");
            }

            if (inline.Href is not null
                && (!Uri.TryCreate(inline.Href, UriKind.Absolute, out var uri)
                    || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
            {
                throw new ArgumentException("Links must use HTTP or HTTPS.");
            }
        }
    }

    private static List<BulletinItem> ReadItems()
    {
        try
        {
            return JsonConvert.DeserializeObject<List<BulletinItem>>(
                Plugin.Instance.Configuration.NewsJson) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    [GeneratedRegex("^#[0-9a-fA-F]{6}$", RegexOptions.CultureInvariant)]
    private static partial Regex HexColorRegex();

    private static bool IsLocalImageUrl(string url)
    {
        const string prefix = "/Bulletin/Image/";
        if (!url.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var fileName = url[prefix.Length..];
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return Path.GetFileName(fileName) == fileName
            && Guid.TryParseExact(Path.GetFileNameWithoutExtension(fileName), "N", out _)
            && extension is ".png" or ".jpg" or ".webp";
    }

    private static string NormalizePanelHeight(string? value)
    {
        return value is "compact" or "tall" ? value : "standard";
    }
}
