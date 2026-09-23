# GlobalDesk · Localization in Wisej.NET · Module 1

Local lab build for **Module 1 · Preparing an Application for Localization**. The GlobalDesk
dashboard with a welcome heading, a Customers button and a culture preview panel. Nineteen keys in
`Resources/Strings.resx`, a `Texts` helper over `Wisej.Resources.ResourceManager` that marks a
missing key instead of blanking a caption, and three real values formatted against
`Application.CurrentCulture` rather than assembled by hand.
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
| Read the preview panel | A date, a count and an amount, formatted for the session's culture. The status line shows the same three values as they are stored. |
| Compare the two | `2026-03-15` versus `Sunday, March 15, 2026`; `1850.75` versus `$1,850.75`. One is data, the other is presentation. |
| Search `DashboardPage.Designer.cs` for an English sentence | There isn't one. Every caption is assigned from a key in `ApplyTextResources`. |
| Ask `Texts.Get` for a key that does not exist | The caption reads `[That.Key]`. Visible, reportable, and the page still renders. |
| Delete a key from `Strings.resx` and rebuild | The same marker appears where that caption was. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Dashboard whose captions all come from resource keys | `ApplyTextResources` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs) |
| `Resources/Strings.resx` with at least twelve semantic keys | [Strings.resx](GlobalDesk/Resources/Strings.resx) - nineteen of them |
| `Texts` helper that marks a missing key | [Texts.cs](GlobalDesk/Texts.cs) |
| A date, a number and a currency formatted with the session's culture | `UpdateCulturePreview` |
| Lab note separating data from presentation | [docs/DataOrPresentation.md](GlobalDesk/docs/DataOrPresentation.md) |

## Notes for anyone extending the dashboard

- `Wisej.Resources.ResourceManager` takes `(baseName, assembly)`. The base name is the manifest
  name without the culture or the extension: `GlobalDesk.Resources.Strings` for
  `Resources/Strings.resx` in a project whose root namespace is `GlobalDesk`.
- `Application.CurrentCulture` is **per session**. Two users in one server process can read two
  languages at once, which is exactly why a `static` culture field is the bug Module 4 is about.
- Use the standard format specifiers - `D`, `N0`, `C` - and pass the culture. A custom pattern like
  `"dd/MM/yyyy"` is a decision you have taken away from every future market.
- Captions are assigned in a method, not in the designer, because from Module 4 that method is
  called again on every culture change.
- `btnCustomers` uses `AutoSize` with a `MinimumSize`, and the preview uses a `TableLayoutPanel`.
  Fixed widths are how German clips.
- `<NeutralLanguage>en</NeutralLanguage>` in the `.csproj` marks the neutral file as English, so
  the build knows it does not need an `en` satellite assembly.
