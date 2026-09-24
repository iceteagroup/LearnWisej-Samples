# GlobalDesk · Localization in Wisej.NET · Module 1

Local lab build for **Module 1 · Localization Fundamentals: Culture, Language and Resources**.
The GlobalDesk dashboard exactly as the walkthrough shows it: the blue application bar, the
**Welcome to GlobalDesk** heading, the **Customers** navigation button and the **Culture preview**
card with its three rows — Date, Quantity, Amount — under a chip naming the session's culture.
Fourteen keys in `Resources/Strings.resx`, a `Texts` helper over
`Wisej.Resources.ResourceManager` that marks a missing key instead of blanking a caption, and
three real values formatted against `Application.CurrentCulture` rather than assembled by hand.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 1/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6101
```

Open <http://localhost:6101>. In Visual Studio, open `GlobalDesk.slnx` and press F5.

## What to try

| Action | Expected result |
|---|---|
| Read the preview card | `Tuesday, October 14, 2025`, `1,234,567.89` and `$1,850.75` — the three values the walkthrough shows, written by the session's culture. |
| Compare each with what the page stores | `2025-10-14` versus `Tuesday, October 14, 2025`; `1850.75` versus `$1,850.75`. One is data, the other is presentation. |
| Read the chip in the card header | `Application.CurrentCulture = en-US` — the culture that formatted the three rows. |
| Read the status line | `Dashboard ready — 8 keys resolved, 3 values formatted.` Counted, not claimed. |
| Search `DashboardPage.Designer.cs` for an English sentence | There isn't one. Every caption is assigned from a key in `ApplyTextResources`. |
| Ask `Texts.Get` for a key that does not exist | The caption reads `[That.Key]`. Visible, reportable, and the page still renders. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Dashboard whose captions all come from resource keys | `ApplyTextResources` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs) |
| `Resources/Strings.resx` with at least twelve semantic keys | [Strings.resx](GlobalDesk/Resources/Strings.resx) — fourteen of them |
| `Texts` helper that marks a missing key | [Texts.cs](GlobalDesk/Texts.cs) |
| A date, a number and a currency formatted with the session's culture | `UpdateCulturePreview` |
| Lab note separating data from presentation | [docs/DataOrPresentation.md](GlobalDesk/docs/DataOrPresentation.md) |

## Notes for anyone extending the dashboard

- `Wisej.Resources.ResourceManager` takes `(baseName, assembly)`. The base name is the manifest
  name without the culture or the extension: `GlobalDesk.Resources.Strings` for
  `Resources/Strings.resx` in a project whose root namespace is `GlobalDesk`.
- `Application.CurrentCulture` is **per session**. Two users in one server process can read two
  languages at once, which is exactly why a `static` culture field is the bug Module 4 is about.
- Use the standard format specifiers — `D`, `N2`, `C` — and pass the culture. A custom pattern like
  `"dd/MM/yyyy"` is a decision you have taken away from every future market.
- Captions are assigned in a method, not in the designer, because from Module 4 that method is
  called again on every culture change.
- `btnCustomers` uses `AutoSize` with a `MinimumSize`. Fixed widths are how German clips.
- The preview values are monospaced on purpose: the point of the card is to compare separators
  and digit grouping between cultures, and a proportional font hides the difference.
- `Desk` in `DeskTheme.cs` holds the palette and converts the walkthrough's CSS pixel sizes to the
  points Wisej uses for fonts. It is layout, not content: nothing in it is localizable.
- `<NeutralLanguage>en</NeutralLanguage>` in the `.csproj` marks the neutral file as English, so
  the build knows it does not need an `en` satellite assembly.
