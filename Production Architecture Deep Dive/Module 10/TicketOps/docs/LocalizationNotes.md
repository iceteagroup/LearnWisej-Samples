# Deliverable — localization notes & resource files

*Module 10 deliverable · TicketOps Console · `Resources/Strings.resx`, `Resources/Strings.de.resx`,
`Resources/Strings.cs`, `Services/LocalizationService.cs`*

## Two cultures ship: en-US and de-DE

| | en-US | de-DE |
|---|---|---|
| Captions | `Strings.resx` (neutral) | `Strings.de.resx` → satellite `de\TicketOps.resources.dll` |
| Date (`"d"`) | `6/14/2026` | `14.06.2026` |
| Date-time (`"g"`) | `6/1/2026 9:30 AM` | `01.06.2026 09:30` |
| Currency (`"C"`) | `$1,850.00` | `1.850,00 €` |
| Number (`"N1"`) | `12.3` | `12,3` |
| Chip captions | Open · In progress · Blocked · Done | Offen · In Bearbeitung · Blockiert · Erledigt |

The values are the **same** `DateTime`/`decimal`/`double` in `Domain/WorkOrder.cs`. Only the culture
passed to `ToString(format, culture)` changes. `Italiano (Italia)` is in the picker and rejected by the
service (`Rule.CultureNotShipped`): the validation path.

## How a key becomes text

```
screen:  _localization.Text("Column.Due")              // key, never a sentence
service: Strings.ResourceManager.GetString(key, culture) // de-DE → de → neutral (.NET fallback chain)
```

1. **Where the strings live** — `Resources/Strings.resx` holds every user-visible string, keyed
   `Area.Name` (`Dashboard.Title`, `Column.Due`, `Status.InProgress`, `Rule.DoneCannotAdvance`, …).
   `Strings.de.resx` holds the German values for the same keys. The csproj pins the manifest names
   (`TicketOps.Resources.Strings.resources`, `….Strings.de.resources`) so the `ResourceManager` base name
   in `Strings.cs` cannot drift from the file layout.
2. **The typed accessor** — `Resources/Strings.cs` wraps the `ResourceManager`: `Strings.ActionFailed` and
   `Strings.AppTitle` resolve for the request thread's UI culture (the shape every module's `ReportFailure`
   uses); `Strings.Get(key, culture)` / `TryGet` take an explicit culture.
3. **The service** — `LocalizationService` owns the session's `CultureInfo` (from `SessionContext.Culture`),
   resolves keys with it and formats with it. It is plain .NET: no Wisej type, so
   `new LocalizationService(new SessionContext { Culture = "de-DE" }, log).FormatCurrency(1850m)` is a
   unit test.
4. **The screen** — after the service accepts a culture, `OperationsDashboard` sets
   `Application.CurrentCulture = culture` (Wisej.NET carries it to the client locale and to every later
   request of this session), re-runs `ApplyLocalizedText()` and re-binds the same snapshot. No reload.

## Fallbacks are a feature, not an exception

| Situation | What the operator sees | What the server log says |
|---|---|---|
| key missing in **every** file | `[Key.Name]` | `WARN [Service] LocalizationService.Text: missing: key '…' …` |
| key missing **only in German** | the neutral English text | `WARN [Service] LocalizationService.Text: untranslated: key '…' missing in de-DE …` |
| malformed `{0}` pattern | the raw pattern | `ERROR [Service] LocalizationService.Format: pattern for '…' is malformed — shown raw` |
| culture not shipped (`it-IT`) | banner *The language "it-IT" is not shipped with this build — staying on de-DE.* (in the current language) | — (an expected result, not a fault) |

"Untranslated" is detected precisely — `ResourceManager.GetResourceSet(culture, true, false)` for the
culture and its parent — not by comparing the German and English strings (which would flag `Status`
= `Status`).

## Decisions worth writing down

- **Data is not translated; UI text is.** Work-order titles ("Repair loading dock pump") are operator-entered
  data and stay as entered in every culture. The walkthrough video translates them for effect; a real
  console would need per-language data columns, which is a data-model decision, not a resource file.
- **Currency follows the culture, the amount does not convert.** `1850m` renders `$1,850.00` and
  `1.850,00 €` — the classic pitfall of `"C"`. A production console stores the currency code with the
  amount and formats with a `NumberFormatInfo` for that currency; the culture then only decides
  separators and symbol placement. Called out here because the video shows exactly this behaviour.
- **Set text and formatting together.** The culture switch runs `ApplyLocalizedText()` *and* `Bind()`:
  German words next to US-formatted dates would look broken, which is the applied guide's warning.
- **Test in the real layout.** The widest translated strings were checked against their controls:
  `In Bearbeitung` (chip, 132 px), `Nächster Status` and `Details öffnen…` (170 px buttons), the German
  footer sentence (712 px). No clipping in either theme.
- **Per session, never static.** `SessionContext.Culture`, the `LocalizationService` and its
  `CultureInfo` are created per session in `AppComposition`. `CultureInfo.DefaultThreadCurrentCulture`
  is deliberately *not* touched — it is process-wide and would leak one operator's language to everyone.
