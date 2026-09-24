# GlobalDesk · Localization in Wisej.NET · Module 4

Local lab build for **Module 4 · Switching the Language at Runtime**. `culture: auto` in
`Default.json`, a language picker whose handler does nothing but assign
`Application.CurrentCulture`, and an `Application.CultureChanged` handler that refreshes the
resource-backed captions, reformats the culture-sensitive values and rebuilds the
designer-localized editor - so every route into a culture change gets the same refresh.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 4/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6104
```

Open <http://localhost:6104>.

## What to try

| Action | Expected result |
|---|---|
| Pick **de-DE** | Everything changes at once - captions, values, and the editor, which the `CultureChanged` handler rebuilt for you. No second button press. |
| Pick **de-AT** | German text, **Austrian** numbers: `12 500` not `12.500`, and `€ 1.850,75` not `1.850,75 €`. There is no `Strings.de-AT.resx`; the text fell back to `de`. |
| Read the culture row | `de-AT - Deutsch (Österreich) (Greift zurück auf de)`. The fallback is on screen instead of being a mystery. |
| Open <http://localhost:6104/?lang=de-AT> | The session starts in Austrian German and the picker is already on `de-AT` - its handler never ran. |
| Open the app in a second browser and switch the language in one | Nothing moves in the other. That is the test for a static culture field. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `Default.json` with `culture: auto`, plus when a fixed culture is right | [Default.json](GlobalDesk/Default.json) and [docs/CultureSwitch.md](GlobalDesk/docs/CultureSwitch.md) |
| Picker handler setting `Application.CurrentCulture` from `CultureInfo.GetCultureInfo` | `cboCulture_SelectedIndexChanged` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs) |
| `CultureChanged` handler calling `ApplyTextResources` and `UpdateCulturePreview` | `Application_CultureChanged` |
| The designer-localized editor recreated after the switch | `CreateEditor`, called from the same handler |
| Verification through `?lang=`, and why culture must not be static | [docs/CultureSwitch.md](GlobalDesk/docs/CultureSwitch.md) |

## Notes for anyone extending the switch

- **The picker assigns the culture and stops.** Everything else is behind `CultureChanged`, so the
  URL parameter, a saved preference and a support tool all get the same refresh. Refreshing inline
  works from exactly one button.
- **Unsubscribe in `Dispose`.** `Application.CultureChanged` outlives the page; a page that forgets
  is kept alive by it for the rest of the session. This sample does it in the designer's `Dispose`,
  beside the components.
- `SyncCultureCombo` needs a re-entry flag, because assigning `SelectedItem` raises
  `SelectedIndexChanged` again. It also adds an unlisted culture rather than leaving the combo
  blank - a French browser is not an error.
- **Never copy the culture into a `static` field.** It is per session, and a static one means one
  user's click changes another user's screen. It never reproduces with one developer and one
  browser.
- Keep a language-region culture with no resource file of its own in the picker. `de-AT` makes
  fallback visible, and visible fallback is the only kind you can reason about.
