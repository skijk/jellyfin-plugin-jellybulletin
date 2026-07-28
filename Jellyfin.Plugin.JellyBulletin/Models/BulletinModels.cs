namespace Jellyfin.Plugin.JellyBulletin.Models;

/// <summary>
/// A published or draft bulletin.
/// </summary>
public sealed class BulletinItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public List<BulletinBlock> Blocks { get; set; } = [];

    public DateTimeOffset PublishedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public bool IsPublished { get; set; }
}

/// <summary>
/// A paragraph or list in a bulletin.
/// </summary>
public sealed class BulletinBlock
{
    public string Type { get; set; } = "paragraph";

    public List<BulletinInline> Content { get; set; } = [];

    public List<List<BulletinInline>> Items { get; set; } = [];
}

/// <summary>
/// A constrained run of rich text.
/// </summary>
public sealed class BulletinInline
{
    public string Text { get; set; } = string.Empty;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underline { get; set; }

    public string? Color { get; set; }

    public string? Href { get; set; }
}

/// <summary>
/// Payload used by the administration page.
/// </summary>
public sealed class SaveBulletinsRequest
{
    public int VisibleItemCount { get; set; } = 5;

    public List<BulletinItem> Items { get; set; } = [];
}

/// <summary>
/// Public response for the home screen widget.
/// </summary>
public sealed class BulletinResponse
{
    public int VisibleItemCount { get; set; }

    public List<BulletinItem> Items { get; set; } = [];
}
