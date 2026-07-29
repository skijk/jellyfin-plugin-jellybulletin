# Changelog

All notable changes to JellyBulletin are documented here.

## 0.3.10.0 — Public beta

- Remove macOS resource-fork files from the release archive.
- Prevent Jellyfin from attempting to load an invalid `._Jellyfin.Plugin.JellyBulletin.dll`.
- Retain the rich-text persistence fixes from 0.3.9.0.

## 0.3.9.0 — Public beta

- Preserve Safari inline styles when serializing rich text.
- Retain bold, italic, underline and color formatting after saving and reload.

## 0.3.8.0 — Public beta

- Round the actual fitted image element instead of only its container.
- Preserve rounded image corners for wide, tall and square contain-fit images.

## 0.3.7.0 — Public beta

- Round inset bulletin image corners to match the surrounding panel.
- Apply proportional corner radii on desktop, mobile and in previews.

## 0.3.6.0 — Public beta

- Add balanced inner spacing around bulletin images.
- Match the image spacing in the editor preview and responsive layout.

## 0.3.5.0 — Public beta

- Automatically enable a bulletin when a publish time is configured.
- Enforce scheduled activation on both the admin page and server.

## 0.3.4.0 — Public beta

- Normalize equivalent UTC timestamp formats before save verification.
- Prevent false “server did not return the saved changes” errors for schedules.

## 0.3.3.0 — Public beta

- Replace the local-only **Apply changes** action with **Save bulletin**.
- Persist and verify bulletin edits immediately without a second save action.

## 0.3.2.0 — Public beta

- Replace inconsistent browser-native date controls with stable text fields.
- Validate the explicit local format `YYYY-MM-DD HH:mm` before saving.

## 0.3.1.0 — Public beta

- Fix invalid-value errors in publish and unpublish date/time fields.
- Show the browser timezone used to interpret scheduling values.

## 0.3.0.0 — Public beta

- Add configurable automatic rotation and rotation intervals.
- Add an accessible pause/start control to the home screen carousel.
- Respect reduced-motion preferences by starting automatic movement paused.
- Add image alternative text.
- Add live overflow guidance for content that requires scrolling.
- Add drag-and-drop announcement ordering.
- Add a single pinned top announcement.
- Add optional publish and unpublish scheduling.
- Follow the active Jellyfin theme in the widget and editor preview.
- Improve the compact carousel and layout preview.
- Use **Bulletin** as the Dashboard menu label.

## 0.2.0.0–0.2.10.0 — Private preview

- Introduce the home screen widget, rich-text editor and optional images.
- Add pasted and dropped image uploads with unused-image cleanup.
- Add stable widget mounting, edit persistence verification and direct
  Dashboard navigation.
- Add automatic sliding, fixed-height layout and responsive image fitting.

## 0.1.0.0–0.1.3.0 — Initial development

- Add the initial Jellyfin 10.11.11 plugin and File Transformation integration.
- Correct startup compatibility and Jellyfin keyboard shortcut handling.
