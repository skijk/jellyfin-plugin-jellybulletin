using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.JellyBulletin.Models;

/// <summary>
/// Payload received from the File Transformation plugin.
/// </summary>
public sealed class PatchRequestPayload
{
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }
}
