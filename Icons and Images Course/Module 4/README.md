# IconDesk · Icons & Images in Wisej.NET · Module 4

Local lab build for **Module 4 · Wisej.NET Icon Packs and NuGet**. An `IconCompare` page that
draws the same five concepts - Save, Delete, Search, User, Settings - from two official packs side
by side, so the choice between them is made by looking. One column is filled the way the Visual
Studio image explorer fills it; the other is assigned from the pack's own catalog in C#.

Modules 1 to 3 are carried forward unchanged; the application opens on the page this module
builds. This folder is the complete application at this point in the course, with its own solution
and port.

## Run it

```bash
cd "Icons and Images Course/Module 4/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6204
```

Open <http://localhost:6204>.

## The screen

`IconDesk — IconCompare`: a header row (**CONCEPT**, **WISEJ-4-FONTAWESOME**,
**WISEJ-4-MATERIALDESIGN**), five concept rows each showing the icon and the file name the pack
actually served, the count chip and the sentence about the two recoloured icons, and the
one-paragraph recommendation.

## What to try

| Action | Expected result |
|---|---|
| Read a row | The same concept, twice, with the file name each pack uses for it. `floppy-o.svg` and `save-button.svg` are the same idea drawn by two different hands. |
| Look at Delete and Settings | Delete is red and Settings is teal: both carry a `?color=` suffix with a theme colour name. Nothing else on the page was touched. |
| Press the **10 icons · 2 packs · 1 page** chip | `btnCompare_Click` rebuilds the Material Design column from the catalog, re-applies the two colour suffixes, and checks every source against the pack assembly's manifest. The sentence beside it reports the result - and names any control whose source does not resolve. |
| Reload with the network panel open | Ten `resource.wx/<assembly>/<icon>.svg` requests the first time, all from the browser cache the second. Cost is per icon used, not per pack installed. |
| Misspell one of the five designer sources and press the chip again | The sentence turns red and names the control. Nothing throws; the control is simply blank, which is exactly why the page checks. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Two official packs referenced through NuGet | `PackageReference` entries in [IconDesk.csproj](IconDesk/IconDesk.csproj) |
| Five concept rows with a caption beside every icon | `BuildRow` in [IconComparePage.Designer.cs](IconDesk/IconComparePage.Designer.cs) |
| One column as the image explorer writes it | the literal `resource.wx` assignments in the designer file |
| One column from the pack catalog in C# | `AssignFromCatalog` in [IconComparePage.cs](IconDesk/IconComparePage.cs) |
| Two icons recoloured with `?color=` | `Recolour` |
| `btnCompare_Click` rebuilding both columns and reporting | `btnCompare_Click` / `Describe` / `Resolves` |
| The recommendation and the evidence | `lblRecommendation`, [docs/IconFamily.md](IconDesk/docs/IconFamily.md) |

## Notes for anyone extending the comparison

- A pack is **one assembly** holding every icon as an embedded SVG, addressed as
  `resource.wx/<assembly>/<icon>.svg`. Nothing is copied into the project, so five forms using one
  icon still ship a single copy of the artwork.
- The catalog (`Wisej.Ext.FontAwesome.Icons` has 519 fields, `Wisej.Ext.MaterialDesign.Icons` 423)
  is the better habit: a mistyped name is a compile error rather than a blank control. The picker
  is what you reach for while laying a screen out.
- Wisej-4 is pinned to **4.1.4** because the packs depend on it; pinning 4.1.0 beside them fails
  the restore with `NU1605: Detected package downgrade`.
- `?color=` needs a colour name the theme really defines - `invalid` and `info` resolve, `error`
  and `activeText` do not, and an unknown name is a silent no-op. See `docs/IconFamily.md`.
- The packs used here are **FontAwesome** and **MaterialDesign**, the two official Wisej-4 packs
  available to this build. The lesson video names `Wisej-4-BootstrapIcons` and
  `Wisej-4-TablerIcons`; those packages are not published for Wisej-4, so the lab's instruction
  "two contrasting official packs" is honoured with the two that exist.
