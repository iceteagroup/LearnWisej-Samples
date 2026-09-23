# Module 4 lab note - choosing a primary icon family

Two official Wisej-4 packs are installed in this project:

```xml
<PackageReference Include="Wisej-4-FontAwesome" Version="4.1.4" />
<PackageReference Include="Wisej-4-MaterialDesign" Version="4.1.4" />
```

Neither copies a file into the project. Each pack is a single assembly with every icon as an
embedded SVG, and every icon is addressed by URL:

```
resource.wx/<assembly name>/<icon file>.svg
```

## What the two routes look like

The FontAwesome row is assigned in `IconComparePage.Designer.cs`, which is exactly what the Visual
Studio image explorer writes when you pick an icon out of an installed pack:

```csharp
this.picFaSave.ImageSource     = "resource.wx/Wisej.Ext.FontAwesome/floppy-o.svg";
this.picFaDelete.ImageSource   = "resource.wx/Wisej.Ext.FontAwesome/trash.svg";
this.picFaSearch.ImageSource   = "resource.wx/Wisej.Ext.FontAwesome/search.svg";
this.picFaUser.ImageSource     = "resource.wx/Wisej.Ext.FontAwesome/user.svg";
this.picFaSettings.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/cog.svg";
```

The Material Design row is assigned from the pack's own catalog in `IconComparePage.cs`:

```csharp
using Md = Wisej.Ext.MaterialDesign.Icons;
...
this.picMdSave.ImageSource = Md.SaveButton;          // resource.wx/Wisej.Ext.MaterialDesign/save-button.svg
```

Both produce the same string. The difference is when you find out you were wrong: the catalog is
519 `public static readonly string` fields for FontAwesome and 423 for Material Design, so a
mistyped name is a compile error. A mistyped picker string is a blank control at run time.

Use the picker while laying a screen out, and the catalog for anything assigned in code.

## Naming, and why it matters more than it should

The five concepts do not have five obvious names:

| Concept | FontAwesome | Material Design |
|---|---|---|
| Save | `FloppyO` → `floppy-o.svg` | `SaveButton` → `save-button.svg` |
| Delete | `Trash` → `trash.svg` | `RubbishBinDeleteButton` → `rubbish-bin-delete-button.svg` |
| Search | `Search` → `search.svg` | `SearchingMagnifyingGlass` → `searching-magnifying-glass.svg` |
| User | `User` → `user.svg` | `UserAccountBox1` → `user-account-box-1.svg` |
| Settings | `Cog` → `cog.svg` | `SettingsCogwheelButton` → `settings-cogwheel-button.svg` |

FontAwesome names the *thing drawn* in one or two words. Material Design names it descriptively and
inconsistently - `UserAccountBox1` has a sibling `UserAccountBox`, and neither name tells you which
is the one you saw in the picker. Searching that catalog for "delete" finds
`RoundDeleteButton` and `RubbishBinDeleteButton`; searching it for "settings" finds three.

## Recolouring

**Recolour the two Delete icons** puts the same suffix on one icon from each family and cycles
through `?color=highlight`, `?color=invalid`, a literal `?color=#7d5ae0`, and no suffix at all.

Both families accept it, every time. That is not luck: pack artwork is drawn as flat single-colour
shapes precisely so it can be recoloured, which is the practical advantage over a folder of
hand-drawn SVGs where some pieces recolour and some - like this project's own multi-coloured
`logo.svg` - do not. Verified by reading the inlined SVG back out of the page: with no suffix every
icon carries the theme's icon colour, `#5F5F5F` under Bootstrap-4; with `?color=highlight` the two
Delete icons switch to the theme's highlight blue and the other eight do not move.

## The browser-cache evidence

Opening the page produces exactly ten requests, one per icon:

```
GET /resource.wx/Wisej.Ext.FontAwesome/floppy-o.svg                  200
GET /resource.wx/Wisej.Ext.FontAwesome/trash.svg                     200
GET /resource.wx/Wisej.Ext.FontAwesome/search.svg                    200
GET /resource.wx/Wisej.Ext.FontAwesome/user.svg                      200
GET /resource.wx/Wisej.Ext.FontAwesome/cog.svg                       200
GET /resource.wx/Wisej.Ext.MaterialDesign/save-button.svg            200
GET /resource.wx/Wisej.Ext.MaterialDesign/rubbish-bin-delete-button.svg  200
GET /resource.wx/Wisej.Ext.MaterialDesign/searching-magnifying-glass.svg 200
GET /resource.wx/Wisej.Ext.MaterialDesign/user-account-box-1.svg     200
GET /resource.wx/Wisej.Ext.MaterialDesign/settings-cogwheel-button.svg   200
```

One request per icon, not one per pack and not one per control. Navigating away and back does not
repeat them - the browser serves them from its own cache, because they are ordinary cacheable URLs
with stable addresses. The cost of an icon pack is therefore *the icons you actually use*, once
per browser, and installing a 519-icon pack to use twelve of them costs twelve requests.

That is also the answer to the obvious worry about installing two packs: the assemblies are
reference-time weight, not download weight. Nothing ships to the browser that no control asked for.

## Recommendation

**FontAwesome is the primary family for IconDesk.** Use Material Design only where FontAwesome has
no icon for a concept, and record each such exception in the project's icon policy.

The reason is not artwork quality - both rows above are perfectly usable, and the Material Design
shapes are arguably the more modern. It is naming and searchability. A developer looking for
"delete" in FontAwesome finds `Trash` and stops; in Material Design they find several plausible
names and have to open the picker to see which is which. Over a large screen count that difference
turns into inconsistency: two developers pick two different "user" icons and nobody notices for a
release. One family, named predictably, is worth more than the best-looking individual icon.

Mixing two families in the same toolbar is the failure mode to avoid. The comparison page here does
it deliberately, and the two rows are visibly different in weight and corner treatment even though
each row is internally consistent.
