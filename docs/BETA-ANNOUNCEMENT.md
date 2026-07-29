# JellyBulletin Public Beta


JellyBulletin — rich announcements on the Jellyfin home screen (public beta)


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

