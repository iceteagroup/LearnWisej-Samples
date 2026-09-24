# IconDesk · Icons & Images in Wisej.NET · Module 7

Local lab build for **Module 7 · Icon Fonts, HTML Content and Production Icon Strategy**. The
capstone: the IconDesk shell, an **Icon Summary** page carrying one tile per mechanism the course
covered, and a **Customer Actions** page whose grid draws its actions as icon-font glyphs inside
an `AllowHtml` column - the sixth mechanism, and the only one that is not an image at all.

Modules 1 to 6 are carried forward unchanged; the application opens on the shell. This folder is
the complete application at the end of the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 7/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6207
```

Open <http://localhost:6207>.

## The screen

`IconDesk · Icon Summary`: the application map down the left - the seven sections the course
built, in the order it built them - and the summary tiles on the right. **Customer Actions** and
**Icon Summary** are the two sections this module owns and the two the nav switches between; the
other five name where the rest of IconDesk lives.

## What to try

| Action | Expected result |
|---|---|
| Read the five tiles | A theme image, an official pack icon, a custom pack icon, an embedded resource and a recoloured SVG. Each tile prints the source string it was given and carries a tooltip. |
| Press the **Theme: …** chip | The other theme loads and every tile gains a verdict. Four followed the theme and one did not - and the one that did not is the *embedded* logo, which is the opposite of the intuitive answer. |
| Open **Customer Actions** | A grid whose Actions column is `AllowHtml`, with two icon-font glyphs per cell. The glyphs are text: no `Image`, no `ImageSource`, nothing for an image property to report. |
| Click a glyph | The strip below names the `role` the element carried, the order, and what the application would do. Click the cell but not a glyph and it says the role was empty. |
| Switch to the dark theme and look at the glyphs | They read correctly, because `icon-font.css` no longer sets a colour on `.idi` - see `docs/ThemeQA.md`. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Icon-font stylesheet loaded before any `AllowHtml` content | the `<link>` in [Default.html](IconDesk/Default.html), [Images/icon-font.css](IconDesk/Images/icon-font.css) |
| An icon-font action column in a grid | `Actions` / `colActions.AllowHtml` in [SummaryPage.cs](IconDesk/SummaryPage.cs) and its designer |
| `role=` telling one handler which action was clicked | `gridCustomers_CellClick` |
| The five-mechanism summary page | `Tiles` / `BuildTiles` |
| The theme QA pass | `btnThemeChip_Click` / `PaintVerdicts`, [docs/ThemeQA.md](IconDesk/docs/ThemeQA.md) |
| The written icon policy | [docs/IconPolicy.md](IconDesk/docs/IconPolicy.md) |

## Notes for anyone extending the capstone

- **Icon-font glyphs are text.** They are styled by a stylesheet the document loads and rendered
  inside `AllowHtml` content. Nothing about them passes through the theme, so a colour set in CSS
  is a colour no theme can revisit. Do not set `color` on a glyph class; let it inherit.
- Reference the stylesheet from `Default.html`, **not** from C#: it has to be in the document
  before any `AllowHtml` content that uses its classes is rendered.
- `AllowHtml` goes on the **column**, not on the grid. `DataGridViewCellEventArgs` carries
  `RowIndex`, `ColumnIndex`, `X`, `Y`, `Location` and - the useful one - **`Role`**. Put
  `role="edit"` on an element inside an `AllowHtml` cell and `CellClick` reports it, which is how
  a click lands on an action rather than on a row.
- The shell is a two-column `TableLayoutPanel`. `Dock = Left` beside `Dock = Fill` on a Page did
  not shrink the content panel here; a table says how the row is shared without either control
  guessing a width.
- A `Button` with `BorderStyle = None` renders as plain text but does **not** raise `Click` in
  Wisej.NET 4.1.4. Every clickable control on this page is a Button with a border style the
  framework recognises, styled with `CssStyle`.

## Where this sample differs from the lesson video

- The video's QA row marks the official pack icon and the custom pack icon **unchanged** after the
  theme switch. On the running page they change, because both are monochrome SVGs with no fill of
  their own - see `docs/ThemeQA.md`. The sample reports what actually happened.
- The video writes `status-warning.svg?color=error`. `error` is not a Wisej.NET theme colour name
  and resolves to nothing; the sample uses `invalid`.
- The five earlier nav entries are shown, as the video shows them, but are not links: a page with
  no way back is worse than a page with none.
