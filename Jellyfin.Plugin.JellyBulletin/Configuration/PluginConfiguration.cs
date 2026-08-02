using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JellyBulletin.Configuration;

/// <summary>
/// Persistent plugin settings.
/// </summary>
public sealed class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Gets or sets serialized news items.
    /// </summary>
    public string NewsJson { get; set; } = "[]";

    /// <summary>
    /// Gets or sets the number of items displayed on the home screen.
    /// </summary>
    public int VisibleItemCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the home-screen announcement panel height.
    /// </summary>
    public string PanelHeight { get; set; } = "standard";

    /// <summary>
    /// Gets or sets a value indicating whether bulletin images are shown on the home screen.
    /// </summary>
    public bool ShowImages { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether announcements rotate automatically.
    /// </summary>
    public bool AutoRotate { get; set; } = true;

    /// <summary>
    /// Gets or sets the automatic rotation interval in seconds.
    /// </summary>
    public int RotationIntervalSeconds { get; set; } = 9;
}
