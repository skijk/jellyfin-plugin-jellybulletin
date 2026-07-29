# JellyBulletin Public Beta

## Jellyfin Forum

### Title

JellyBulletin — rich announcements on the Jellyfin home screen (public beta)

### Post

JellyBulletin is a new public-beta plugin for Jellyfin Web that lets server
administrators publish announcements directly on the home screen.

It currently supports:

- Bold, italic, underline and colored text
- Clickable HTTP and HTTPS links
- Bullet and numbered lists
- Uploaded, pasted, dropped or externally hosted images
- Three to five announcements in a carousel
- Automatic rotation, manual navigation and pause
- Pinned announcements
- Scheduled publishing and unpublishing
- Theme-aware presentation
- A live layout preview in the administration page

![JellyBulletin beta announcement](images/jellybulletin-home-beta.png)

### Requirements

- Jellyfin Server 10.11.11
- Jellyfin Web or another web-based client
- File Transformation plugin

Native clients that do not render Jellyfin Web are not currently supported.
All published announcements are visible to every authenticated user.

### Installation

First add the File Transformation repository and install File Transformation:

```text
https://www.iamparadox.dev/jellyfin/plugins/manifest.json
```

Then add the JellyBulletin repository:

```text
https://raw.githubusercontent.com/skijk/jellyfin-plugin-jellybulletin-repository/main/catalog-v2.json
```

Install JellyBulletin, restart Jellyfin and open **Dashboard → Plugins →
Bulletin**.

Source, documentation and bug reports:

https://github.com/skijk/jellyfin-plugin-jellybulletin

This is a public beta. Feedback and reproducible bug reports are very welcome.
Please include the Jellyfin version, browser/client, JellyBulletin version,
File Transformation version and relevant Jellyfin logs.

## Reddit

### Title

[Plugin] JellyBulletin public beta — rich announcements on the Jellyfin home screen

### Post

**AI disclosure:** JellyBulletin was developed with extensive assistance from
OpenAI Codex. I directed the product design, requirements and testing, while
Codex assisted with implementation, review, packaging and documentation. The
plugin has been tested on my own Jellyfin server, and the use of AI assistance
is also documented in the project README.

I have released the public beta of JellyBulletin, a plugin for publishing
server news, maintenance notices and other announcements directly on the
Jellyfin home screen.

Features include rich text, links, lists, images, scheduling, pinning and an
automatic announcement carousel. The presentation follows the active Jellyfin
theme and every published announcement is visible to all authenticated users.

It is currently tested with Jellyfin 10.11.11 and targets Jellyfin Web and
web-based clients. File Transformation is required.

Project and instructions:

https://github.com/skijk/jellyfin-plugin-jellybulletin

Plugin repository:

```text
https://raw.githubusercontent.com/skijk/jellyfin-plugin-jellybulletin-repository/main/catalog-v2.json
```

This is beta software, so feedback and bug reports are appreciated.
