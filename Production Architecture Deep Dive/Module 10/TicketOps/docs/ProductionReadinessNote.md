# Production-readiness note — theming & localization

*Module 10 · lab step 8 · TicketOps Console*

## Ready

- **Look and logic are separate.** No colour, pattern or operator-facing sentence lives in `Views/` or
  `Controls/StatusChip`. A rebrand edits the theme / mixin; a new language adds one `.resx`; neither
  touches a handler.
- **Every path is visible and safe.** Success (refresh, theme, culture), progress (walk statuses),
  validation (`it-IT` not shipped), domain rule (finished order), resource gaps (missing / untranslated
  key) and the data outage all end in a localized sentence for the operator and a `[LAYER]` line in the
  trace. `sql01:1433` never reaches a label; `Strings.ActionFailed` does, in the operator's language.
- **Per session, nothing static.** `SessionContext`, `LocalizationService`, `ThemeSwitcher`, the
  repository and the log are built once per session by `AppComposition`. Two browser tabs can run in two
  languages.
- **Build:** `dotnet build -nologo -v q` → 0 warnings, 0 errors for `net10.0-windows` and `net10.0`;
  the German satellite assembly is produced under `de\`.

## Before shipping

| Item | Why | Where |
|---|---|---|
| Decide the scope of a theme switch | `Application.LoadTheme` re-skins **every session** of the process; a per-operator dark mode must use the session-only path (`Application.Theme = …`) and the mixin must be applied to that theme too | `Infrastructure/ThemeSwitcher.cs`, `docs/ThemeSwitch.md` |
| Persist the operator's choices | `SessionContext` lives as long as the session; theme and culture should also be saved to the user profile store and restored in `Program.Main` before the first paint | `ThemeSwitcher.RestoreSaved`, `SessionContext` |
| Store amounts with a currency code | `"C"` follows the culture's currency symbol; `1850m` must not become `1.850,00 €` if it was dollars | `LocalizationService.FormatCurrency` |
| Add the missing translations | `Button.Export` is untranslated on purpose; a build step should diff the key sets of `Strings.resx` and every `Strings.*.resx` | `Resources/` |
| Move the banner colours into the theme | `StatusBanner.ColorFor` is the last literal palette; alias to `primary`/`success`/`warning`/`danger` in the mixin | `Controls/StatusBanner.cs` |
| Ship `Themes/` with the app | the mixin is read from the application's `/Themes` folder; the csproj copies it to the output, verify it in the publish output | `TicketOps.csproj` |
| Test the widest strings per language | done for de; repeat for every new culture in both themes | `docs/LocalizationNotes.md` |
