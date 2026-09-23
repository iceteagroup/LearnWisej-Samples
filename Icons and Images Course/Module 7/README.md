# IconDesk · Icons & Images in Wisej.NET · Module 7

Local lab build for **Module 7 · Icon Fonts and the Capstone**. A `Summary` page that puts every
mechanism the course covered on one screen - a theme image, an official pack icon, a custom pack
icon, an embedded asset and a recoloured SVG - plus the one thing Module 7 adds: icon fonts, in an
`AllowHtml` toolbar and in a `DataGridView` column whose glyphs respond individually to a click.
The Module 1-6 pages are unchanged.
This folder is the complete application at the end of the course.

## Run it

```bash
cd "Icons and Images Course/Module 7/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6207
```

Open <http://localhost:6207> and press **Summary**.

## What to try

| Action | Expected result |
|---|---|
| Click the pencil, the arrow or the cross in a grid row | The status line names the action **and** the row: `delete on floppy-o.svg (cell x=67, y=15)`. The `role` attribute on the glyph is what makes that possible. |
| Click the actions cell but between glyphs | `Role was empty` - the page can tell the difference between "clicked an action" and "clicked the cell". |
| Switch to **BootstrapDark-4** | Every image-based icon turns light. Every icon-font glyph stays dark grey. That is the finding, and it is left in on purpose. |
| Press **Run the theme QA pass** | The page reports its own image sources and points at the finding. |
| Look at the toolbar card | One `Label`. No `Image`, no `ImageSource`, no `ImageList` - the glyphs are text. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Icon-font stylesheet referenced from `Default.html` | [Default.html](IconDesk/Default.html) and [Images/icon-font.css](IconDesk/Images/icon-font.css) |
| An `AllowHtml` toolbar rendering font glyphs | `BuildIconFontToolbar` in [SummaryPage.cs](IconDesk/SummaryPage.cs) |
| A grid column with icon-font actions that respond to the clicked element | `BuildGrid` / `gridAssets_CellClick`, and `colActions.AllowHtml` in the designer |
| A summary page showing all five image mechanisms | `ShowEveryMechanism` |
| Production icon policy | [docs/IconPolicy.md](IconDesk/docs/IconPolicy.md) |
| QA pass in a light and a dark theme | [docs/ThemeQA.md](IconDesk/docs/ThemeQA.md) |

## Notes for anyone extending the capstone

- **`DataGridViewCellEventArgs.Role`** is how a click lands on an action rather than on a row. Put
  a `role="edit"` attribute on the element inside the `AllowHtml` cell and Wisej.NET reports it
  back. `e.X` and `e.Y` give the position inside the cell if a layout needs them. Verified live.
- `AllowHtml` goes on the **column**, not the grid.
- **Icon fonts are the one mechanism a theme switch cannot reach.** Glyphs are text; the theme
  never sees them. Do not set `color` on the glyph classes - let them inherit from the surrounding
  text, which the theme does control. The sample deliberately keeps the broken version so the QA
  pass has something to find.
- The stylesheet is referenced from `Default.html` rather than from C#, because it has to be in the
  document before any `AllowHtml` content that uses its classes is rendered.
- `icon-font.css` uses Unicode glyphs from the system font rather than shipping a bespoke webfont,
  so the lab runs with no internet access and nothing binary is smuggled in. Swapping in a real
  family is one `@font-face` rule and a change of `content` values; the file says so at the top and
  nothing else on the page changes.
