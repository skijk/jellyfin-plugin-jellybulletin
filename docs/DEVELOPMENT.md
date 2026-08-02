# Development builds

Development builds contain work intended for the next JellyBulletin release.
They may change before being promoted to the stable catalog.

## Development catalog

Use this repository URL in **Dashboard → Plugins → Repositories**:

```text
https://raw.githubusercontent.com/skijk/jellyfin-plugin-jellybulletin-repository/main/catalog-dev.json
```

Configure either the stable or development JellyBulletin catalog, not both.
After switching catalogs, refresh the plugin catalog, install the offered
version and restart Jellyfin.

The stable catalog remains:

```text
https://raw.githubusercontent.com/skijk/jellyfin-plugin-jellybulletin-repository/main/catalog-v2.json
```

## 0.3.21 layout test checklist

Version 0.3.21 adds a global **Adaptive** panel height and per-bulletin title
appearance. Useful combinations to verify are:

- Adaptive height with a short text-only bulletin and a Compact title
- Adaptive height with the title hidden on the home screen
- Adaptive height with longer rich text, lists and links
- Adaptive height with landscape and portrait images
- Compact, Standard and Tall fixed-height modes for upgrade compatibility
- Desktop and narrow/mobile browser widths
- The administration preview before saving and the home screen after saving

Existing configurations should open as **Standard**, and existing bulletins
should use **Prominent** titles. A visually hidden title must still be visible
in the administration list and available to assistive technology on the home
screen.
