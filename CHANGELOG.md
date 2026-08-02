# Changelog

All notable changes to JellyBulletin are documented here.

## 0.3.21.0 — Development

- Add an Adaptive panel height that follows the displayed content.
- Add Prominent, Compact and Hidden title styles per bulletin.
- Keep titles available in administration and as accessible labels when hidden visually.

## 0.3.20.0

- Publish the current text-only, compact two-item, and external image URL features as a stable release.
- Clarify that File Transformation is required and JellySpotlight placement integration is optional.

## 0.3.19.0 — Public beta

- Restore HTTP and HTTPS image URLs alongside locally uploaded images.
- Supersede the overly restrictive image handling in 0.3.18.0.

## 0.3.18.0 — Public beta

- Prevent browser Basic Auth prompts caused by protected external bulletin images.
- Display only images uploaded to JellyBulletin's local image storage.
- Keep image support without third-party authentication requests or tracking.

## 0.3.17.0 — Public beta

- Use the full panel width in text-only mode.
- Show two separate announcements at once in text-only mode on wide screens.
- Keep one announcement at a time on mobile.
- Reduce text-only panel height and whitespace.

## 0.3.16.0 — Public beta

- Add a global **Show images on the home screen** setting.
- Preserve uploaded artwork while using a lower text-focused layout when images are hidden.

## 0.3.15.0 — Public beta

- Present the plugin logo on a centered 16:9 catalog canvas with safe spacing.

## 0.3.14.0 — Public beta

- Add Compact, Standard and Tall announcement panel heights.
- Apply the selected height consistently to the carousel and live preview.
- Preserve Standard as the default for existing installations.

## 0.3.13.0 — Public beta

- Preserve bullet and numbered lists nested inside browser-generated wrappers.
- Keep paragraph and list boundaries instead of flattening them into one text run.
- Support the differing contenteditable DOM structures used by major browsers.

## 0.3.12.0 — Public beta

- Capture the structured rich-text model as editing and formatting occur.
- Preserve the model before Save Bulletin moves focus away from the editor.
- Avoid rebuilding saved content from a browser-normalized, blurred editor.

## 0.3.11.0 — Public beta

- Preserve the editor selection when using formatting controls, colors and links.
- Serialize Safari's computed bold, italic and underline styles.
- Keep rich formatting and list structure through the save-and-reload cycle.

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
