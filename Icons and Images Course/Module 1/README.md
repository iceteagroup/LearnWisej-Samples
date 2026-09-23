# IconDesk · Icons & Images in Wisej.NET · Module 1

Local lab build for **Module 1 · Image Sources, Icons and the ImageList**. One command panel whose
four buttons carry their icons four different ways - a server-side `System.Drawing.Image`, a named
theme image on `ImageSource`, and two controls sharing one `ImageList` through `ImageKey` - plus a
diagnostics label that reads every control back through `IImage` and names the mechanism actually
in play.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 1/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6201
```

Open <http://localhost:6201>. In Visual Studio, open `IconDesk.slnx` and press F5.

## What to try

| Action | Expected result |
|---|---|
| Look at the diagnostics label | Each control is named with the mechanism behind its icon. `btnOpen` reports an `Image`, `btnSave` an `ImageSource`, the rest an `ImageList` entry. |
| Press **Replace the image under the key** | The bin becomes a grey cross on **both** `btnDelete` and `lblDeleteEcho`. Neither control was assigned to - only the entry under the key `"delete"` changed. |
| Press it again | Both go back. The key is the contract; the picture behind it is free to change. |
| Press **Set Image and ImageSource on one button** | The status line reports `Image still set afterwards: False`. Assigning `ImageSource` cleared `Image`; they feed one slot. |
| Press **Inspect through IImage** | The report is rebuilt from the live controls, so it always describes the current state rather than the startup state. |
| Open the browser network panel and reload | `btnOpen` and the `ImageList` pictures arrive as `component.wx` requests (server-rendered PNG). The theme icon does not - it comes inline with the theme. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Command panel with Open, Save, Print and Delete | [CommandPage.Designer.cs](IconDesk/CommandPage.Designer.cs) |
| `btnOpen` from a server-side `System.Drawing.Image`, `btnSave` from a named theme image | `LoadCommandIcons` in [CommandPage.cs](IconDesk/CommandPage.cs) |
| `btnPrint` and `btnDelete` sharing one `ImageList` through `ImageKey` | `LoadCommandIcons`, plus `imagesCommands` in the designer |
| Diagnostics label reading each control through `IImage` | `Describe` / `ReportMechanisms` in [CommandPage.cs](IconDesk/CommandPage.cs) |
| Replacing the image under a key, and setting two image properties at once | `btnSwapDelete_Click` / `btnSetBoth_Click` |
| Lab note | [docs/ImageMechanisms.md](IconDesk/docs/ImageMechanisms.md) |

## Notes for anyone extending the panel

- `Image` and `ImageSource` are **mutually exclusive**. Assigning `ImageSource` sets `Image` to
  null, verified in `btnSetBoth_Click`. Nothing warns you, so a lost icon is usually a second
  assignment somewhere else rather than a missing file.
- A control fed from an `ImageList` still returns a picture from `Image`. Test `ImageList` and
  `ImageKey` **before** `Image` or every keyed control looks like an image-object control.
- `ImageList.Images["key"]` is read-only as an assignment target. It returns the live
  `ImageListEntry`; set `entry.Image` to repoint the key.
- `Image.FromFile` keeps the file locked for the lifetime of the image, which is the last thing a
  server application wants. `LoadServerImage` reads the bytes and builds the image from a
  `MemoryStream` instead.
- `Application.MapPath("Images/open.png")` resolves whether the application runs from `bin/` or
  from a published folder. The `Images` folder is copied to the output by the `.csproj`, because
  the browser also fetches some of these files by relative URL in later modules.
