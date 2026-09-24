# IconDesk · Icons & Images in Wisej.NET · Module 1

Local lab build for **Module 1 · The Wisej.NET Image Model: Image, ImageSource and ImageList**.
One command panel whose four buttons carry their icons three different ways - a server-side
`System.Drawing.Image`, a named theme image on `ImageSource`, and two buttons sharing one
`ImageList` through `ImageKey` - plus `lblDiagnostics`, which reads each button back through
`IImage` and names the mechanism actually in play.

This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 1/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6201
```

Open <http://localhost:6201>. In Visual Studio, open `IconDesk.slnx` and press F5.

## The screen

A 40-pixel application bar (`IconDesk — Command panel` and the three window glyphs), a
**Commands** heading, the four command buttons with the glyph above the caption, the
`lblDiagnostics` card, and a status strip. Nothing else.

## What to try

| Action | Expected result |
|---|---|
| Press **Open**, **Save**, **Print**, **Delete** in turn | Each click adds one line to the diagnostics card naming that button's mechanism: `Image`, `ImageSource`, then two `ImageList + Key` lines. With all four printed the status strip turns green. |
| Press **Delete** again | The picture stored under the key `"delete"` is replaced. The bin becomes a crossed circle although no image property was assigned on any control - the button holds the key, not the picture. |
| Press **Delete** once more | A second image property is set on the same button. The diagnostics line flips to `ImageSource` and the status strip turns red: the key did not merely stop deciding, assigning the source **cleared** it. |
| Open the browser network panel and reload | `btnOpen` and the two `ImageList` pictures arrive as `component.wx` requests (server-rendered PNG). The theme icon does not - it arrives inline with the theme as a `data:image/svg+xml` URL. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Command panel with Open, Save, Print and Delete | [CommandPage.Designer.cs](IconDesk/CommandPage.Designer.cs) |
| `btnOpen` from a server-side `System.Drawing.Image`, `btnSave` from a named theme image | `LoadCommandIcons` in [CommandPage.cs](IconDesk/CommandPage.cs) |
| `btnPrint` and `btnDelete` sharing one `ImageList` through `ImageKey` | `LoadCommandIcons`, plus `imagesCommands` in the designer |
| Diagnostics label reading each control through `IImage` | `DescribeIcon` / `WriteDiagnostics` in [CommandPage.cs](IconDesk/CommandPage.cs) |
| Replacing the image under a key, and setting a second image property on one button | `ReplaceDeleteEntry` / `TakeTheSlotFromTheKey` |
| Lab note | [docs/ImageMechanisms.md](IconDesk/docs/ImageMechanisms.md) |

## Notes for anyone extending the panel

- `Image` and `ImageSource` are **mutually exclusive**, and so are `ImageSource` and
  `ImageList`/`ImageKey`. Assigning an image source clears the others - verified in
  `TakeTheSlotFromTheKey`. Nothing warns you, so a lost icon is usually a second assignment
  somewhere else rather than a missing file.
- A control fed from an `ImageList` still returns a picture from `Image`. Test `ImageList` and
  `ImageKey` **before** `Image` or every keyed control looks like an image-object control.
- `ImageList.Images["key"]` is read-only as an assignment target. It returns the live
  `ImageListEntry`; set `entry.Image` to repoint the key.
- A repointed key does not reach the browser on its own: Wisej.NET serves each control's picture
  from a URL stamped with that **control's** version. `RepaintControlsOn` finds every control on
  the same list and key and writes the same key back, which moves the version without changing
  what the control points at.
- A button's icon is sized by the theme (18 pixels in Bootstrap-4) unless an `ImageList` supplies
  an `ImageSize`. `imagesCommands.ImageSize` is set to 18 so all four glyphs match.
- `Image.FromFile` keeps the file locked for the lifetime of the image, which is the last thing a
  server application wants. `LoadServerImage` reads the bytes and builds the image from a
  `MemoryStream` instead.
- `Application.MapPath("Images/open.png")` resolves whether the application runs from `bin/` or
  from a published folder. The `Images` folder is copied to the output by the `.csproj`, because
  the browser also fetches some of these files by relative URL in later modules.
