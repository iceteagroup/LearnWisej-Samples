# Module 4 lab note - choosing a primary icon family

Written from the running IconCompare page and the browser network panel.

## The two packs, and the two ways of reaching them

Both packs are single assemblies whose icons are embedded SVGs, addressed as
`resource.wx/<assembly>/<icon>.svg`. Referencing one grows the deployment by one file; nothing is
copied into IconDesk.

The FontAwesome column carries the literal strings the Visual Studio image explorer writes into
`IconCompare.Designer.cs`:

```csharp
this.picFaSave.ImageSource     = "resource.wx/Wisej.Ext.FontAwesome/floppy-o.svg";
this.picFaDelete.ImageSource   = "resource.wx/Wisej.Ext.FontAwesome/trash.svg";
this.picFaSearch.ImageSource   = "resource.wx/Wisej.Ext.FontAwesome/search.svg";
this.picFaUser.ImageSource     = "resource.wx/Wisej.Ext.FontAwesome/user.svg";
this.picFaSettings.ImageSource = "resource.wx/Wisej.Ext.FontAwesome/cog.svg";
```

The Material Design column is assigned from the pack's own catalog in C#:

```csharp
this.picMdSave.ImageSource     = Md.SaveButton;
this.picMdDelete.ImageSource   = Md.RubbishBinDeleteButton;
this.picMdSearch.ImageSource   = Md.SearchingMagnifyingGlass;
this.picMdUser.ImageSource     = Md.UserAccountBox1;
this.picMdSettings.ImageSource = Md.SettingsCogwheelButton;
```

Reading the two side by side is the point of the exercise. They produce the same kind of string;
the catalog turns a typo into a compile error, and the picker turns "what is this icon called"
into a preview.

The five concepts also show how differently two families name the same idea: `floppy-o.svg`
against `save-button.svg`, `trash.svg` against `rubbish-bin-delete-button.svg`, `cog.svg` against
`settings-cogwheel-button.svg`. Mixing families means carrying two vocabularies as well as two
stroke weights.

## Recolouring, and the names that quietly do nothing

Both families draw flat monochrome shapes with no `fill` of their own, so the fill Wisej.NET
injects is what paints them and the `?color=` suffix always lands. Decoded out of the running
page:

| Control | Source | Inlined `fill` |
|---|---|---|
| `picFaDelete` | `…/trash.svg?color=invalid` | `#DC3444` |
| `picMdSettings` | `…/settings-cogwheel-button.svg?color=info` | `#17A1B8` |

The value has to be a colour name the theme really defines. An earlier pass used `?color=error`,
which Bootstrap-4 does not define: the inlined SVG came back with a literal `fill="error"`, the
browser ignored it, and the icon kept its previous colour. No warning, no exception. `invalid` is
the Bootstrap-4 name for that red; `activeText` is not a Wisej.NET colour name either.

Test a recolour on the real artwork before a screen depends on it.

## Cost

Cost is **per icon used**, not per pack installed. Ten icons produce exactly ten small
`resource.wx` requests on the first view and none afterwards - the second load comes entirely from
the browser cache. A pack of five hundred icons is fine; a screen that draws five hundred at once
is not.

## The failure path

A misspelled `resource.wx` file name is not a compile error and not an exception. The control is
simply blank. `Resolves` asks the pack assembly's manifest whether the file exists and
`btnCompare_Click` names any control that fails, because a blank rectangle is not a bug report.

## The recommendation

**Wisej-4-FontAwesome** as IconDesk's primary family:

- it covers all five concepts with one consistent stroke weight;
- its artwork is monochrome SVG that takes the `?color=` suffix, so status colouring is a source
  string rather than a second file;
- its names are short and map onto the vocabulary IconDesk already uses;
- it ships as one assembly with one licence to record.

Material Design stays referenced because Module 3's gallery uses it, and because keeping a second
family visible is what made the difference arguable rather than assumed. Record both packs' source
and licence, keep the `PackageReference` versions in source control, and test a version change
before it reaches production.

## Where this sample differs from the lesson video

The video's columns are headed `Wisej-4-BootstrapIcons` and `Wisej-4-TablerIcons`. Those packages
are not published for Wisej-4; the two official packs that exist are FontAwesome and
MaterialDesign, and they are what the lab compares.
