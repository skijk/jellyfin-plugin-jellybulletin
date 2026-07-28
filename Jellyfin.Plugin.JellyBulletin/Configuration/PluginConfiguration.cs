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
}
