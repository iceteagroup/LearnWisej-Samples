# IconDesk · Icons & Images in Wisej.NET · Module 6

Local lab build for **Module 6 · Building Your Own Icon Pack**. A second project,
`IconDesk.Icons`, holds twelve embedded SVGs and a static `AppIcons` catalog of `resource.wx`
strings - the same shape the official packs from Module 4 have. The application consumes it both
ways: three buttons carry literal pack URLs written into the designer file, and the whole set below
is built from `AppIcons.All`, so adding an icon to the pack adds it to the page with no edit.
The Module 1-5 pages are unchanged.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 6/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6206
```

Open <http://localhost:6206> and press **Project icon pack**. In Visual Studio, open
`IconDesk.slnx` - it now carries two projects.

## What to try

| Action | Expected result |
|---|---|
| Look at the two rows | The three designer buttons and the twelve catalog icons come from the same assembly by the same URL shape. |
| Press **Recolour three catalog icons** | Warning, Info and Delete take `?color=highlight`, then `invalid`, then `hotTrack`. The other nine do not move. |
| Press **Check the catalog against the assembly** | `12 embedded icons, 12 catalog entries, no drift.` Delete an icon file, rebuild, and it names the one that went missing. |
| Type `AppIcons.` in the editor | The whole pack, with IntelliSense. A mistyped name is a compile error, not a blank control. |
| Open the network panel | Twelve `resource.wx/IconDesk.Icons/*.svg` requests, one per icon, cached afterwards. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Class library with twelve embedded SVGs | [IconDesk.Icons.csproj](IconDesk.Icons/IconDesk.Icons.csproj) and `IconDesk.Icons/Resources/` |
| Static `AppIcons` catalog of named `resource.wx` strings | [AppIcons.cs](IconDesk.Icons/AppIcons.cs) |
| Some icons assigned in the designer, others from the catalog | `btnAdd`/`btnEdit`/`btnRemove` in [IconPackPage.Designer.cs](IconDesk/IconPackPage.Designer.cs), `BuildCatalogRow` in [IconPackPage.cs](IconDesk/IconPackPage.cs) |
| Three icons verified with the colour suffix, plus the SVG cleanup they needed | `btnRecolour_Click` and [ICON-PACK-RULES.md](IconDesk.Icons/ICON-PACK-RULES.md) |
| Naming and versioning rules, and how an application overrides an icon | [ICON-PACK-RULES.md](IconDesk.Icons/ICON-PACK-RULES.md) |

## Notes for anyone extending the pack

- **A line icon must stroke with `currentColor`.** Wisej.NET recolours an SVG by setting `fill` and
  `color` on the root element; it does not rewrite `stroke` attributes. An icon drawn with a
  literal stroke colour renders fine, accepts a `?color=` suffix without complaint, and never
  changes colour. This was the one real cleanup the twelve icons needed.
- No `width`/`height` on the root, only `viewBox`, or the icon ignores the size of the control it
  is in.
- The pack has **no Wisej dependency**. It is a plain class library, so the catalog can be tested
  without a session and the pack can be consumed by any application.
- The URL is the **assembly** name: `resource.wx/IconDesk.Icons/add.svg`. Qualified, so nothing on
  a deployment's disk can replace a shipped icon - see the Module 5 note for the evidence and for
  the deliberate exception.
- `ProjectReference` paths in the `.csproj` use forward slashes here. The drift check in
  `btnVerify_Click` is the unit test this pack would ship in a real project.
