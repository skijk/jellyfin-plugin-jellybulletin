# JellyBulletin

**News and announcements for Jellyfin.**

JellyBulletin displays rich news and service messages on the Jellyfin home
screen. All published messages are visible to every user. The newest item is
open by default and the latest three to five items remain selectable.

## Current development target

- Jellyfin 10.11.11
- .NET 9
- Jellyfin Web and web-based clients

## Runtime dependency

The File Transformation plugin is required to inject the home screen panel into
Jellyfin Web. JellyBulletin serves all JavaScript and CSS from the Jellyfin server;
it does not depend on a public CDN.

The plugin is in early development and is not ready for installation yet.
