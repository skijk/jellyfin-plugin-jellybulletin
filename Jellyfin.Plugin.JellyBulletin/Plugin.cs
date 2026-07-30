using System.Globalization;
using Jellyfin.Plugin.JellyBulletin.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.JellyBulletin;

/// <summary>
/// JellyBulletin plugin.
/// </summary>
public sealed class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public override string Name => "JellyBulletin";

    public override string Description => "Announcements for Jellyfin Web. Requires File Transformation; JellySpotlight placement integration is optional.";

    public override Guid Id => Guid.Parse("6ad77d9a-e157-4ca2-82e7-a114f86a5f50");

    public static Plugin Instance { get; private set; } = null!;

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                DisplayName = "Bulletin",
                EnableInMainMenu = true,
                MenuIcon = "campaign",
                EmbeddedResourcePath = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.Configuration.configPage.html",
                    GetType().Namespace)
            }
        ];
    }
}
