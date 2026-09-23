# IconDesk cookbook - Wisej-4 4.1.4 / .NET 10

Everything here was established by running the labs and reading the result back, not from
documentation. Each entry says how.

## Structure

One project per module under `Module N/IconDesk`, namespace `IconDesk`, ports 6201-6207. Module 6
adds a second project, `IconDesk.Icons`, and the solution carries both. Wisej-4 is pinned to
**4.1.4**: the official icon packs depend on it, and pinning 4.1.0 beside them fails the restore
with `NU1605: Detected package downgrade`.

## The two pipelines

`Image` hands Wisej.NET a `System.Drawing.Image` in server memory, which it sends to the browser as
PNG under a `component.wx` URL. `ImageSource` hands it a string the client resolves: a relative
URL, an absolute URL, a theme name, or a `resource.wx` path into an assembly.

**They are mutually exclusive.** Assigning `ImageSource` sets `Image` to null. Verified in Module 1
by assigning both and reading the control back: `Image still set afterwards: False`. Nothing warns
you, and nobody writes both lines together - one is in the designer and the other in a helper that
runs later, which is why the reported symptom is "the icon disappeared after we added theming".

A control fed from an `ImageList` also **returns a picture from `Image`**. Test `ImageList` and
`ImageKey` before `Image` or every keyed control looks like an image-object control.

`ImageList.Images["key"]` is read-only as an assignment target. It returns the live
`ImageListEntry`; set `entry.Image` to repoint the key. Every control holding that key follows,
because they hold the key and not the picture.

## `PictureBox.LoadAsync`

Sets **`ImageSource`**, not `Image`, and lets the **browser** perform the request. Nothing is
downloaded or decoded on the server and `Image` is still null when `LoadCompleted` fires. Verified
in Module 2 against a deliberately slow local endpoint:

```
LoadCompleted after 2733 ms. The picture is on ImageSource ("slow/photo.jpg") and Image is
still null - the browser fetched it, not the server.
```

Consequences: the URL must be reachable **from the browser**, so anything behind server-side
credentials has to be fetched in C# and assigned to `Image` instead. `ShowLoader` is not switched
off for you - do it in `LoadCompleted`.

`PictureBoxSizeMode` is `Normal, StretchImage, AutoSize, CenterImage, Zoom, Cover`.

## SVG recolouring - the rule that surprises everyone

Wisej.NET does not hand the browser a URL for an SVG image source. It reads the file and **inlines
it** as a `data:image/svg+xml;base64` background, and on the way through it decides whether the
artwork is a recolourable *icon* or a picture.

- **One meaningful `fill`** (white and `none` do not count) → treated as an icon. The fill is
  replaced with the theme's icon colour, or with `?color=`.
- **Several fills** → treated as artwork and passed through untouched.
- **`stroke` attributes are never rewritten.**

Established in Module 3 by decoding the inlined SVG out of the page under both themes.
`status-ok.svg` (one green fill plus a white stroke) came back with `fill="#A0B5BD"`;
`logo.svg` came back with all six of its fills intact.

Two practical consequences:

- Where the file came from is **irrelevant**. An absolute URL to a static file is recoloured just
  the same as a theme image. The intuitive rule - "themes own theme images, files are files" - is
  wrong.
- A **line** icon only recolours if it strokes with `currentColor`, because Wisej sets `fill` and
  `color` on the root element and the stroke picks up the injected `color`:

```xml
<svg viewBox="0 0 24 24" fill="#298AE5" style="color: rgb(41,138,229); fill: rgb(41,138,229);">
  <path fill="none" stroke="currentColor" ... />
</svg>
```

A literal `stroke="#5A6B7D"` renders correctly, accepts a `?color=` suffix without complaint, and
never changes colour. That was the one real cleanup the Module 6 pack needed.

`?color=` takes a theme colour name (`highlight`, `hotTrack`, `invalid`, `grayText`, `windowText`)
or a literal. A name follows the theme; a literal does not, and is almost always wrong.

## Themes

`Application.Theme` is a **`ClientTheme` object**, not a string. Read `Application.Theme.Name`;
switch with `Application.LoadTheme("BootstrapDark-4")`.

Named theme images in Bootstrap-4 are a short list: `icon-search`, `icon-settings`, `icon-print`,
`icon-check`, `icon-close`, `icon-refresh`, `icon-calendar`, `icon-columns`, the `messagebox-*`
family and a handful of chrome pieces. There is no theme name for every concept an application
needs, which is the gap the icon packs fill.

## Icon packs

Official packs are single assemblies with every icon as an embedded SVG, addressed as
`resource.wx/<assembly>/<icon>.svg`. Each exposes a static `Icons` catalog of constants -
`Wisej.Ext.FontAwesome.Icons` has 519 fields, `Wisej.Ext.MaterialDesign.Icons` 423 - so a mistyped
name is a compile error rather than a blank control.

Cost is per icon used, not per pack installed: ten icons produce exactly ten cacheable requests on
first view and none afterwards.

## `resource.wx` and overrides

Manifest names are `<RootNamespace>.<folder>.<file>`; the URL strips `<RootNamespace>.Resources.`.
Use `LogicalName="<Root>.Resources.%(Filename)%(Extension)"` to keep sources in a differently named
folder. `Include="Resources**"` is not a valid glob and fails with `CS1566` - use
`Include="Assets/**/*"`.

| URL | Resolves to |
|---|---|
| `resource.wx/IconDesk/logo.svg` | **always** the embedded resource |
| `resource.wx/logo.svg` | a file of that name in the **application root** if present, else the embedded resource |

Established in Module 5 by moving one file around while fetching both URLs. A `Resources/`
subfolder beside the application is **not** searched. Qualify anything that must be exactly what
you shipped; leave unqualified anything a deployment is meant to replace, and write down which is
which.

Resource URLs are cached by the browser. If the bytes behind one change, change the URL too.

## Icon fonts

Glyphs are **text**, styled by a stylesheet the document loads, rendered inside `AllowHtml`
content. Nothing about them passes through the theme, so a colour set in CSS is a colour no theme
can revisit - the Module 7 QA pass found exactly that, with dark-grey glyphs on a dark background
beside image icons that had correctly turned light. Do not set `color` on glyph classes; let them
inherit from the surrounding text.

Reference the stylesheet from `Default.html`, not from C#: it has to be in the document before any
`AllowHtml` content that uses its classes is rendered.

## Grid cells

`AllowHtml` goes on the **column**, not the grid. `DataGridViewCellEventArgs` carries `RowIndex`,
`ColumnIndex`, `X`, `Y`, `Location` and - the useful one - **`Role`**. Put `role="edit"` on an
element inside an `AllowHtml` cell and `CellClick` reports it, which is how a click lands on an
action rather than on a row. Verified live: `delete on floppy-o.svg (cell x=67, y=15)`.

## Layout notes

An action bar that gains a button per module will overflow a fixed `Panel`. Use a
`FlowLayoutPanel` with `WrapContents = true` and drop the absolute `Location` assignments.

## Other gotchas

- `Microsoft.AspNetCore.Http.HttpContext` has to be written out in full in `Startup.cs`:
  `Wisej.Core` defines an `HttpContext` too.
- `Image.FromFile` keeps the file locked for the lifetime of the image. Read the bytes and build
  from a `MemoryStream`.
- `Application.MapPath("Images/x.png")` resolves whether the app runs from `bin/` or a published
  folder.
- A `--` inside an XML comment breaks the `.csproj` with `MSB4025`.

## Reflection trick for API questions

Build the project once, then load `System.Drawing.Managed.dll` followed by `Wisej.Framework.dll`
from the **output folder** with an `AssemblyResolve` handler pointing at that folder. Loading them
from the NuGet cache fails to resolve types. `GetTypes()` throws on missing ASP.NET Core
references; use `GetType(name)`.
