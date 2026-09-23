# Module 3 lab note - what a theme switch actually moves

Run the Icon gallery, switch between `Bootstrap-4` and `BootstrapDark-4`, and watch the five
pictures. The result is not the one most people predict, and the reason is worth the page.

## The prediction, and what really happened

The obvious rule is "the theme owns theme images; files are files". Under that rule the two
`icon-*` pictures move and the three file-based ones stay.

What actually happened:

| Control | Image source | Moved? |
|---|---|---|
| `picTheme` | `icon-search` | yes |
| `picThemeSecond` | `icon-settings` | yes |
| `picRecoloured` | `Images/pin.svg?color=highlight` | yes |
| `picAbsolute` | `http://localhost:6203/Images/status-ok.svg` | **yes** |
| `picProject` | `Images/logo.svg` | no |

Four of the five moved, including one that is nothing but an absolute URL to a static file on
disk. The prediction is wrong.

## Why, established from the rendered DOM

Wisej.NET does not hand the browser the URL for an SVG image source. It reads the file and
**inlines it** as a `data:image/svg+xml;base64` background, and on the way through it decides
whether the artwork is a recolourable *icon* or a picture.

`status-ok.svg` is written as a green disc with a white tick. Decoded out of the page under the
dark theme it arrives as:

```xml
<svg viewBox="0 0 24 24" fill="#A0B5BD" style="color: rgb(160,181,189); fill: rgb(160,181,189);">
  <circle cx="12" cy="12" r="10" fill="#A0B5BD"/>
  <path fill="none" stroke="#ffffff" .../>
</svg>
```

The green is gone. Its single meaningful fill was replaced with `#A0B5BD`, the dark theme's icon
colour, and white was left alone.

`logo.svg`, inlined from the same page at the same moment, is untouched:

```
fill="#1565d8" fill="#0d47a1" fill="#ffffff" fill="#ffffff" fill="#ffd166" fill="#1fae5a"
```

No root `fill`, no injected `style`, six colours still there.

**The rule that falls out:** an SVG with essentially one colour (white and `none` do not count) is
treated as an icon and takes the theme's icon colour. Artwork with several colours is passed
through as-is. Where the file lives - theme name, relative URL, absolute URL - has nothing to do
with it.

## What that means when you are choosing assets

- A monochrome SVG is the right choice for interface icons **because** it will be recoloured. It
  will look correct in a theme nobody has written yet.
- If you need an asset to keep its own colours - a logo, a flag, a product shot - give it more
  than one colour, or use a raster. A one-colour logo will be quietly repainted by the first dark
  theme somebody switches on, and it will look like a bug in the theme rather than in the asset.
- `?color=` makes the decision explicit rather than leaving it to the default. `?color=highlight`
  names a colour from the theme's palette, so it moves with the theme; `?color=#7d5ae0` pins a
  literal, which is almost always the wrong answer. Press **Cycle the colour suffix** to walk
  through `highlight`, `hotTrack`, `invalid` and one literal and watch the difference.
- The suffix needs artwork it can recolour. `pin.svg` is one path, one fill, no stroke and no
  gradient, and it is commented as being that way on purpose. Pointing the suffix at `logo.svg`
  does nothing, which is the same finding from the other direction.

## What the designer Image Selector writes

Every image source on this page is a plain string in `IconGalleryPage.Designer.cs`, which is what
the picker produces - it never generates an `Image` object for these:

```csharp
this.picTheme.ImageSource = "icon-search";
this.picProject.ImageSource = "Images/logo.svg";
this.picRecoloured.ImageSource = "Images/pin.svg?color=highlight";
this.picThemeSecond.ImageSource = "icon-settings";
```

That is the practical argument for using the picker rather than assigning images in code: the
result is readable, greppable and reviewable in a diff, and a mistyped theme name shows up as a
blank control rather than as a runtime exception.

`picAbsolute` is the exception on this page. Its URL is built in the constructor from
`Application.Url` so the sample runs on whatever port you start it on; a real project would let
the picker write the literal address.

Two icon-pack assets belong in this gallery as well. The packs are installed in Module 4, and the
`IconCompare` page there covers them.
