# IconDesk · Icons & Images in Wisej.NET · Module 4

Local lab build for **Module 4 · Official Icon Packs**. Two official Wisej-4 icon packs are
installed, and an `IconCompare` page draws the same five concepts - Save, Delete, Search, User and
Settings - from both families so the difference is impossible to argue about. One row is assigned
the way the Visual Studio image explorer writes it, the other from the pack's own catalog in C#.
The Module 1-3 pages are unchanged; the action bar now wraps so every page stays reachable.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 4/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6204
```

Open <http://localhost:6204> and press **Icon packs**.

## What to try

| Action | Expected result |
|---|---|
| Compare the two rows | Same five concepts, visibly different weight and corner treatment. Each row is internally consistent; mixing them in one toolbar would not be. |
| Press **Recolour the two Delete icons** | Both Delete icons take `?color=highlight`, then `?color=invalid`, then a literal, then nothing. Pack artwork accepts the suffix every time. |
| Press **Report the pack resource URLs** | Ten `resource.wx/<assembly>/<icon>.svg` strings. None of them names a file inside this project. |
| Open the network panel and reload | Exactly ten requests, one per icon. Navigate away and back: no repeats - they come from the browser cache. |
| Read the two assignment styles | The designer row is five literal strings; the catalog row is `Md.SaveButton` and friends, where a typo is a compile error. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Two official icon-pack NuGet packages | [IconDesk.csproj](IconDesk/IconDesk.csproj) |
| Five concepts from both packs | [IconComparePage.Designer.cs](IconDesk/IconComparePage.Designer.cs) and [IconComparePage.cs](IconDesk/IconComparePage.cs) |
| Five icons chosen the way the image explorer writes them | the `picFa*` assignments in the designer file |
| Two icons recoloured with the colour suffix | `btnRecolour_Click` |
| Recommendation and cache evidence | [docs/IconFamily.md](IconDesk/docs/IconFamily.md) |

## Notes for anyone extending the packs

- Both packs depend on **Wisej-4 4.1.4**. Pinning Wisej-4 to 4.1.0 alongside them fails the
  restore with `NU1605: Detected package downgrade`, which is why this course pins 4.1.4
  throughout.
- The catalog classes are `Wisej.Ext.FontAwesome.Icons` (519 fields) and
  `Wisej.Ext.MaterialDesign.Icons` (423 fields). Every field is a `resource.wx` string, so they can
  be used anywhere an `ImageSource` is accepted and concatenated with a `?color=` suffix.
- Installing a pack costs nothing at run time beyond the icons a control actually asks for. Each is
  one cacheable request with a stable URL.
- Material Design's names are descriptive and inconsistent (`UserAccountBox1` has a sibling
  `UserAccountBox`). Search the catalog before assuming an icon is missing.
