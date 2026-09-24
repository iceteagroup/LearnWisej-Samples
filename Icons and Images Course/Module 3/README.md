# IconDesk · Icons & Images in Wisej.NET · Module 3

Local lab build for **Module 3 · Theme Images, URLs, SVG Color and the Designer Image Picker**.
An `IconGallery` page with five slots - a named theme image, a relative URL, an absolute URL, an
icon-pack `resource.wx` source and the same kind of source with a `?color=` suffix - and a theme
switch that says which of them moved.

Modules 1 and 2 are carried forward unchanged; the application opens on the page this module
builds. This folder is the complete application at this point in the course, with its own solution
and port.

## Run it

```bash
cd "Icons and Images Course/Module 3/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6203
```

Open <http://localhost:6203>.

## The screen

`IconDesk — IconGallery`: a **THEME** row with a Light and a Dark chip, then five cards. Each card
carries the picture, the caption naming its mechanism, and the source string exactly as it appears
in `IconGallery.Designer.cs`. After the first theme switch each card also carries its verdict.

## What to try

| Action | Expected result |
|---|---|
| Read the five source strings | `icon-print`, `Images/company-logo.svg`, the absolute `…/cdn/users/42.png`, `resource.wx/…/android-logo.svg` and `…/save-button.svg?color=highlight`. Nothing on the page decodes an image on the server. |
| Press **Dark theme** | The page repaints and every card gains a verdict. Three followed the theme, two did not - and not the three you would guess. |
| Press **Light theme** | Everything goes back. The verdicts stay, because the point of the exercise is the comparison. |
| Hover the two pack cards | The tooltip shows the full `resource.wx` string; the card prints the elided form the video uses. |
| Open the network panel | Two `resource.wx/Wisej.Ext.MaterialDesign/*.svg` requests, one `Images/company-logo.svg`, one `cdn/users/42.png`. The theme image costs no request of its own. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Five gallery slots with captions naming the mechanism | `layoutSlots` in [IconGalleryPage.Designer.cs](IconDesk/IconGalleryPage.Designer.cs), [GallerySlot.cs](IconDesk/GallerySlot.cs) |
| A named theme image and a project SVG by relative URL | `AssignImageSources` in [IconGalleryPage.cs](IconDesk/IconGalleryPage.cs) |
| A complete external address, and two icon-pack assets | `AssignImageSources`; the pack comes from the `Wisej-4-MaterialDesign` reference in the `.csproj` |
| `?color=` with a theme colour name, and artwork that ignores it | `RecolouredIcon`; `Images/company-logo.svg` is the artwork that does not move |
| A theme switch, and the note listing what changed | `SwitchTo` / `ApplyPalette`, [docs/ThemeSwitch.md](IconDesk/docs/ThemeSwitch.md) |

## Notes for anyone extending the gallery

- `Application.Theme` is a **`ClientTheme` object**, not a string. Read `Application.Theme.Name`;
  switch with `Application.LoadTheme("BootstrapDark-4")`.
- The page paints its own surface, panel and text colours from `GalleryPalette` rather than
  inheriting them. That is deliberate: if the whole page redrew itself from the theme there would
  be nothing to compare. Only the five image sources are left to the theme.
- **Where the file came from does not decide whether it follows the theme.** Wisej.NET inlines an
  SVG image source as a `data:image/svg+xml` background and injects `fill` and `color` on the root
  element. Artwork that fixes its own colours is unaffected; artwork that does not is repainted,
  whatever URL it arrived through. See `docs/ThemeSwitch.md`.
- `?color=` needs a colour name the theme really defines. `highlight`, `hotTrack`, `invalid`,
  `info`, `grayText` and `windowText` resolve. `activeText` and `error` do not - Wisej.NET passes
  the unknown name through to the SVG's `fill`, the browser ignores it, and the icon quietly keeps
  the colour it already had.
- The absolute-URL slot is served by this application from its own `cdn/` folder so the lab works
  with no outbound access. "Another host" is a property of the URL, and the production caution -
  availability, authentication, CORS and CSP belong to whoever owns that host - is unchanged.
