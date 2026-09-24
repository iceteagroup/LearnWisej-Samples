# IconDesk · Icons & Images in Wisej.NET · Module 5

Local lab build for **Module 5 · Embedded Resources and resource.wx**. A `ResourceLab` page with
five assets compiled into `IconDesk.dll` and served through `resource.wx`: one source qualified
with the assembly name, four left unqualified, one replaced at run time by a file deployed beside
the application, and one misspelled on purpose.

Modules 1 to 4 are carried forward unchanged; the application opens on the page this module
builds. This folder is the complete application at this point in the course, with its own solution
and port.

## Run it

```bash
cd "Icons and Images Course/Module 5/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6205
```

Open <http://localhost:6205>.

## The screen

`IconDesk — ResourceLab`: one button, the sentence about `Build Action`, five asset tiles, and the
`lblResourceReport` status strip. Each tile carries the picture, the file name and the shape of
the URL it came from.

## What to try

The page has a single button, and its caption names the next step.

| Press | Expected result |
|---|---|
| **Show resources** | All five tiles resolve. The strip turns green: *Ready — 5 embedded assets served by resource.wx*. In the network panel that is five `resource.wx` requests, one per asset, answered out of `IconDesk.dll`. |
| **Deploy an override** | The lab writes `status-ok.svg` into the application root. The tile turns purple and its pill reads *deployed file wins* - and `logo.svg` does **not** move, because its source names the assembly. The title bar says *(customer deployment)*. |
| **Remove the override** | The file is deleted and `status-ok.svg` falls back to the embedded resource with no code change. |
| **Misspell one URL** | `picWarning` is pointed at `status-warnning.svg`. Nothing throws. The tile shows a dashed **404** box and the strip turns red, because a blank rectangle is not a bug report. |
| **Reset** | Back to the start. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Five assets marked Embedded Resource | the `EmbeddedResource` item in [IconDesk.csproj](IconDesk/IconDesk.csproj), files in `Assets/` |
| All five shown on a ResourceLab page | `AssignSources` in [ResourceLabPage.cs](IconDesk/ResourceLabPage.cs), tiles in the designer |
| One source qualified with the assembly name | `QualifiedLogo` |
| One asset overridden by a deployed file | `DeployOverride` / `RemoveOverride` |
| A missing asset reported rather than left blank | `Misspell`, `AssetTile.TileState.Missing` |
| Troubleshooting note | [docs/ResourceTroubleshooting.md](IconDesk/docs/ResourceTroubleshooting.md) |

## Notes for anyone extending the lab

- Manifest names are `<RootNamespace>.<folder>.<file>`, and the URL strips
  `<RootNamespace>.Resources.`. `LogicalName="IconDesk.Resources.%(Filename)%(Extension)"` keeps
  the sources in an `Assets/` folder while serving them as `resource.wx/IconDesk/<file>`.
- `Include="Resources**"` is **not** a valid glob and fails with `CS1566`. Use `Include="Assets/**/*"`.
- `resource.wx/IconDesk/logo.svg` is **always** the embedded resource.
  `resource.wx/status-ok.svg` is a file of that name in the **application root** if one is
  present, and the embedded resource otherwise. A `Resources/` subfolder beside the application is
  not searched.
- Qualify anything that must be exactly what you shipped; leave unqualified anything a deployment
  is meant to be able to replace - and write down which is which, because the override folder now
  needs the same protection as the binaries.
- Resource URLs are cached by the browser, correctly. If the bytes behind one change, change the
  URL too: `Bust()` appends a version so the lab's own swap is visible.
- `Assets/logo.svg` carries **two** fills on purpose. Wisej.NET inlines an SVG image source and
  injects a fill on the root; artwork with more than one meaningful fill is passed through
  untouched, so the brand colours survive. A single-fill version of the same drawing would be
  repainted by the theme.
