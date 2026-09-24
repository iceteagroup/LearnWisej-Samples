# Module 3 lab note - what a theme switch actually moves

Read off the running IconGallery page under `Bootstrap-4` and `BootstrapDark-4`, by decoding the
inlined SVG out of the page under both themes. Not from the documentation.

## The five image-source strings, as `IconGallery.Designer.cs` and the page both print them

| Control | Source string | Kind |
|---|---|---|
| `picThemeImage` | `icon-print` | theme key |
| `picProjectSvg` | `Images/company-logo.svg` | relative URL |
| `picAbsoluteUrl` | `http://localhost:6203/cdn/users/42.png` | absolute URL |
| `picPackOne` | `resource.wx/Wisej.Ext.MaterialDesign/android-logo.svg` | resource source |
| `picPackTwo` | `resource.wx/Wisej.Ext.MaterialDesign/save-button.svg?color=highlight` | resource source + colour |

## What moved

| Slot | Light theme | Dark theme | Verdict |
|---|---|---|---|
| `picThemeImage` | `fill="#5F5F5F"` | `fill="#A0B5BD"` | followed the theme |
| `picProjectSvg` | unchanged | unchanged | unchanged |
| `picAbsoluteUrl` | a PNG | a PNG | unchanged |
| `picPackOne` | `fill="#5F5F5F"` | `fill="#A0B5BD"` | **followed the theme** |
| `picPackTwo` | `fill="#298AE5"` | the dark theme's highlight | followed the theme |

Three of the five moved, and the third one is the surprise.

## The rule, which is not the intuitive one

The intuitive rule is "the theme owns theme images, and everything else is a fixed file". It is
wrong.

Wisej.NET does not hand the browser a URL for an SVG image source. It reads the file and **inlines
it** as a `data:image/svg+xml;base64` background, and on the way through it writes `fill` and
`color` on the root element. What happens next is decided by the artwork:

- `android-logo.svg`, straight out of an official pack, has **no `fill` on any of its paths**.
  The injected root fill is therefore what paints it, and it changes with the theme even though
  nothing about `resource.wx/Wisej.Ext.MaterialDesign/android-logo.svg` mentions a theme.
- `company-logo.svg` sits in this project's own `Images` folder - the most "fixed file" source
  there is - and does not move, because every path sets `fill="none"` and strokes with a literal
  colour. Strokes are never rewritten.

So: **where the file came from is irrelevant.** What decides is whether the artwork leaves its own
colours to be filled in. An absolute URL to somebody else's static file is recoloured exactly the
same way a theme image is.

That is also the practical rule for drawing icons. A line icon only follows the theme if it
strokes with `currentColor`:

```xml
<svg viewBox="0 0 24 24" fill="#1565d8">
  <path fill="none" stroke="currentColor" ... />
</svg>
```

A literal `stroke="#6b7c90"` renders correctly, accepts a `?color=` suffix without complaint, and
never changes colour. `company-logo.svg` is written that way on purpose - a brand mark is artwork,
not an icon.

## The colour suffix, and the names that do not work

`?color=` is applied to the source string, so it works on a theme image name, a relative URL and a
`resource.wx` source alike.

The value has to be a colour name the theme really defines. `highlight` resolved to
`rgb(41, 138, 229)` under Bootstrap-4 and to the dark theme's highlight under BootstrapDark-4.
`activeText` did not resolve at all: the inlined SVG came back with `fill="activeText"` and
`style="color: activetext;"`, which is a CSS system-colour keyword rather than anything Wisej.NET
chose, so the icon was coloured by the browser and stopped following the theme. `error` behaves
the same way - `invalid` is the Bootstrap-4 name for that colour.

There is no error, no warning and no exception. A mistyped colour name is a silent no-op, which is
the whole argument for previewing a recolour before a screen depends on it.

## Where this sample differs from the lesson video

- The video's gallery labels the pack resource **unchanged** after the theme switch. On the
  running page it is not - see above. The sample reports what actually happened.
- The video writes `save.svg?color=activeText`. `activeText` is not a Wisej.NET theme colour, so
  the sample uses `highlight`, which is.
- The video's absolute URL is `cdn.example.com/users/42.jpg`, a placeholder host. The sample uses
  a real absolute address this application answers, so the slot resolves on a machine with no
  outbound access.
