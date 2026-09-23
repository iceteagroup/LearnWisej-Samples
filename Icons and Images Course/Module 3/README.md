# IconDesk · Icons & Images in Wisej.NET · Module 3

Local lab build for **Module 3 · The Designer Image Selector and Theme Images**. An `IconGallery`
page carrying five image sources of five different kinds - a named theme image, a relative project
SVG, an absolute URL, a monochrome SVG recoloured with the `?color=` suffix, and a second theme
image - plus a theme switch that separates the assets a theme moves from the ones it does not.
The Module 1 command panel and the Module 2 image lab are unchanged.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 3/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6203
```

Open <http://localhost:6203> and press **Icon gallery**.

## What to try

| Action | Expected result |
|---|---|
| Switch the theme to **BootstrapDark-4** | Four of the five pictures change. The multi-coloured project logo does not. |
| Predict which four before you look | Most people say the two `icon-*` images. The absolute URL moves as well, and that is the lesson. |
| Press **Cycle the colour suffix** | The pin walks through `highlight`, `hotTrack`, `invalid` and one literal `#7d5ae0`. The three theme names follow a later theme switch; the literal does not. |
| Press **Report the image sources** | Every source is printed as the designer wrote it, with a verdict on whether the theme moves it and why. |
| Read `IconGalleryPage.Designer.cs` | Five plain strings. The picker never produces an `Image` object for these controls. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Five controls with designer-assigned image sources | [IconGalleryPage.Designer.cs](IconDesk/IconGalleryPage.Designer.cs) |
| One theme image, one project SVG, one absolute URL | `picTheme`, `picProject`, `picAbsolute` |
| Monochrome SVG recoloured with `?color=` and a theme colour name | `picRecoloured`, `btnRecolour_Click` in [IconGalleryPage.cs](IconDesk/IconGalleryPage.cs) |
| Designer excerpt showing the generated strings | quoted in [docs/ThemeSwitch.md](IconDesk/docs/ThemeSwitch.md) |
| Theme-switch note | [docs/ThemeSwitch.md](IconDesk/docs/ThemeSwitch.md) |

The two icon-pack assets the lab also asks for arrive in Module 4, which installs the packs.

## Notes for anyone extending the gallery

- **Wisej.NET inlines SVG image sources** as `data:image/svg+xml;base64` and recolours the ones it
  reads as icons. An SVG with one meaningful fill takes the theme's icon colour; artwork with
  several colours is passed through untouched. Where the file came from is irrelevant - an
  absolute URL to a static file is recoloured just the same.
- That is why `?color=` only does anything useful to monochrome artwork, and why a one-colour logo
  is a trap: the first dark theme repaints it.
- `Application.Theme` is a `ClientTheme` object, not a string. Read `Application.Theme.Name` and
  switch with `Application.LoadTheme("BootstrapDark-4")`.
- Theme colour names for the suffix come from the theme's own `colors` section: `highlight`,
  `hotTrack`, `invalid`, `grayText`, `windowText` and so on.
- The named theme images available in Bootstrap-4 are a short list - `icon-search`,
  `icon-settings`, `icon-print`, `icon-check`, `icon-close`, `icon-refresh`, `icon-calendar` and a
  handful more. There is no theme name for every concept you will need, which is the gap the icon
  packs in Module 4 fill.
