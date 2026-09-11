# Production-readiness note — theming & localization

*Module 10 · lab step 8 · TicketOps Console*

## Ready

- **Look and logic are separate.** No colour, pattern or operator-facing sentence lives in `Views/` or
  `Controls/StatusChip`. A rebrand edits the theme / mixin; a new language adds one `.resx`; neither
  touches a handler.
- **Every path is visible and safe.** Success (theme, culture, next status), validation (`it-IT` not
  shipped), the domain rule (a finished order cannot advance) and unexpected errors all end in a localized
  sentence for the operator; exception details go to the server log only, never to a label.
- **Per session, nothing static.** `SessionContext`, `LocalizationService`, `ThemeSwitcher`, the
  repository and the log are built once per session by `AppComposition`. Two browser tabs can run in two
  languages and two themes.

## Before shipping

| Item | Why | Where |
|---|---|---|
| Persist the operator's choices | `SessionContext` lives as long as the session; theme and culture should also be saved to the user profile store and restored before the first paint | `ThemeSwitcher.RestoreSaved`, `SessionContext` |
| Store amounts with a currency code | `"C"` follows the culture's currency symbol; `1850m` must not become `1.850,00 €` if it was dollars | `LocalizationService.FormatCurrency` |
| Check translations in the build | a build step should diff the key sets of `Strings.resx` and every `Strings.*.resx`; at runtime a gap only shows as an English fallback and a log warning | `Resources/` |
| Move the banner colours into the theme | `StatusBanner.ColorFor` is the last literal palette; alias to `primary`/`success`/`warning`/`danger` in the mixin | `Controls/StatusBanner.cs` |
| Ship `Themes/` with the app | the mixin is read from the application's `/Themes` folder; the csproj copies it to the output, verify it in the publish output | `TicketOps.csproj` |
| Test the widest strings per language | done for de; repeat for every new culture in both themes | `docs/LocalizationNotes.md` |
