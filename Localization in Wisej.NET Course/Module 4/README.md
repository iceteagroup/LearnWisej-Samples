# GlobalDesk · Localization in Wisej.NET · Module 4

Local lab build for **Module 4 · Browser Culture, Session Culture and Runtime Language Switching**.
The GlobalDesk dashboard exactly as the walkthrough shows it: the blue application bar, the
**Welcome to GlobalDesk** heading, the **Customers** button, the **Language** picker on the right,
the **Culture preview** panel with Date / Quantity / Amount, the designer-localized **Customer
editor** panel beside it, and the status strip reporting the session culture.

One selection repaints three things, in one handler: the resource-backed captions, the formatted
values, and the designer-localized editor — which is rebuilt, because that is the only way a
control that read its resources at construction changes language.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 4/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6104
```

Open <http://localhost:6104>. `culture` is `auto` in `Default.json`, so the session starts from the
browser's `Accept-Language`; <http://localhost:6104/?lang=de-DE> in a **new tab** starts a second
session in German while the first stays where it is.

## What to try

| Action | Expected result |
|---|---|
| Pick **Deutsch (Deutschland)** | Bar, heading, button, picker caption, preview captions and all three values change together; the editor panel is rebuilt in German and shows a green **neu erstellt** badge; the strip reads `Sitzungskultur: de-DE`. |
| Watch the editor's buttons | `Speichern` and `Abbrechen` are wider than `Save` and `Cancel`. The widths came from `CustomerEditor.de.resx` with the new instance. |
| Pick **Français (Canada)** | English text — there is no French resource file — with Canadian French formatting: `mercredi 23 septembre 2026`, `1 234 567,89`, `1 850,75 $`. Fallback is per key, and formatting does not come from a resource at all. |
| Open `?lang=de-DE` in a second tab | Two sessions, two cultures, one server process, both open at once. |
| Search for a `static` culture field | There isn't one. The session already stores it. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `culture` on `auto` in `Default.json` | [Default.json](GlobalDesk/Default.json), with a note on when to pin it instead |
| Language `ComboBox` whose captions come from resource keys | `FillLanguagePicker` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs); keys `Language.en-US`, `Language.de-DE`, `Language.fr-CA` |
| `SelectedIndexChanged` that only assigns `Application.CurrentCulture` | `cboLanguage_SelectedIndexChanged` |
| `CultureChanged` handler calling `ApplyTextResources` and `UpdateCulturePreview` | `Application_CultureChanged` |
| The designer-localized editor recreated in the same handler | `CreateEditor` |
| Verified through the `?lang=` parameter | see above |
| Lab note on what a static culture field would do | [docs/CultureSwitch.md](GlobalDesk/docs/CultureSwitch.md) |

## Notes

- The combo handler assigns the culture and **stops**. Everything else hangs off
  `Application.CultureChanged`, so the `?lang=` parameter, a saved preference and a support tool
  all get the same refresh for free.
- The subscription is removed in `Dispose`. `Application.CultureChanged` outlives the page.
- The language list shows each language in **its own** language. A German speaker looking for
  German finds *Deutsch*, not *German*, so those three values are marked invariant.
- The badge on the editor panel states a comparison the page can actually make:
  `editor.BuiltForCulture` against `Application.CurrentCulture.Name`. It never says *recreated*
  unless a rebuild happened.
- The picker lives in its own right-docked column rather than on an anchor. An anchor measures its
  margin from the design-time size, and the browser is never that size.
