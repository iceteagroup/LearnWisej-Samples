# IconDesk · Icons & Images in Wisej.NET · Module 6

Local lab build for **Module 6 · Building Your Own Wisej.NET Icon Pack**. A second project,
`IconDesk.Icons`, holding twelve SVGs as embedded resources and a static `AppIcons` catalog of
`resource.wx` strings - and an `IconPack` page that draws all twelve from that one assembly and
then puts the `?color=` suffix on the three status icons to see which artwork accepts it.

Modules 1 to 5 are carried forward unchanged; the application opens on the page this module
builds. This folder is the complete application at this point in the course, with its own solution
and port, and the solution now carries two projects.

## Run it

```bash
cd "Icons and Images Course/Module 6/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6206
```

Open <http://localhost:6206>. In Visual Studio, open `IconDesk.slnx` - it carries `IconDesk` and
`IconDesk.Icons`.

## The screen

`IconDesk — Pack Gallery`: the toolbar (**Pack gallery**, the `IconDesk.Icons v1.0.0` chip and the
theme chip), the twelve-icon grid four to a row, the load summary, then the **Recolour check**
section with the three status icons at 16, 24, 32 and 48 pixels, and the verdict strip at the
bottom.

## What to try

| Action | Expected result |
|---|---|
| Read the grid | Twelve icons, all of them `resource.wx/IconDesk.Icons/<file>.svg`. The first four carry the literal string the image explorer writes (`designer`); the other eight come from the catalog (`AppIcons`). |
| Look at the Error card | It is grey at every size although its source carries `?color=invalid`, the card is outlined red and the strip says why: the draft artwork strokes with a literal colour. |
| Press **Remove the hard-coded fill** | The card switches to the pack's cleaned `AppIcons.Error`, the icon turns red at all four sizes and the strip turns green. |
| Press the theme chip | Everything repaints. The three checked icons keep the colours their suffixes asked for, because a theme colour **name** resolves again under the new theme. |
| Reload with the network panel open | Twelve `resource.wx` requests the first time, none the second - one request per icon, then the cache. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| A class library with a `Resources` directory of embedded SVGs | [IconDesk.Icons.csproj](IconDesk.Icons/IconDesk.Icons.csproj), `IconDesk.Icons/Resources` |
| A static catalog so callers never type a path | [AppIcons.cs](IconDesk.Icons/AppIcons.cs) |
| Designer and code assignments side by side | `BuildGallery` in [IconPackPage.cs](IconDesk/IconPackPage.cs) |
| Three recolour checks | `BuildRecolourCheck` / `ApplyCheckSources` |
| The pack's naming, recolouring and versioning rules | [ICON-PACK-RULES.md](IconDesk.Icons/ICON-PACK-RULES.md) |

## Notes for anyone extending the pack

- The pack is a **plain class library with no Wisej dependency at all**, so it can be consumed by
  any application and tested without a session. Manifest names come out as
  `IconDesk.Icons.Resources.<file>.svg`, which Wisej.NET serves at
  `resource.wx/IconDesk.Icons/<file>.svg`.
- Every icon in the pack is written to be recolourable: one meaningful `fill` on the root so
  Wisej.NET reads it as an icon, `fill="none"` on every shape so the injected fill cannot paint it
  in, and `stroke="currentColor"` so the strokes take the injected colour. A literal stroke colour
  renders correctly and then never changes again - which is exactly the defect the recolour check
  is built to expose.
- `?color=` needs a colour name the theme really defines. `success`, `info`, `invalid`,
  `highlight` and `hotTrack` resolve; `error` and `activeText` do not, and an unknown name is a
  silent no-op.
- `Assets/status-error-draft.svg` in the **application** is the Error icon as it arrived from the
  designer, kept so the check has something to fail on. It is not part of the pack.
- Cost is per icon used, not per pack installed. A pack of five hundred icons is fine; a screen
  that draws five hundred at once is not.

## Where this sample differs from the lesson video

The video shows the gallery and the recolour check as two separate windows at two URLs. The sample
puts them on one scrolling page with the video's own section headings, because the page has no
navigation of its own and a hidden view switch would be a control the video never shows.
