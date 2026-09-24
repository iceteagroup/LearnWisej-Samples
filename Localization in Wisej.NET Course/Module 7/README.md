# GlobalDesk · Localization in Wisej.NET · Module 7

Local lab build for **Module 7 · Production Localization Architecture, Testing and Capstone**.
The finished GlobalDesk exactly as the walkthrough shows it: the blue application bar, the
navigation rail with the language picker at its foot, the heading **Open tickets this week**, the
three value tiles — **Due date**, **Open amount**, **Resolved** — the ticket grid with
Ticket / Customer / Due / Amount / Status, and the customer record's two actions.

Everything user-visible on it goes through one door, `LocalizationService`. A solution-wide search
for `ResourceManager` returns that file and the designer's own `ComponentResourceManager`, and
nothing else.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 7/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6107
```

Open <http://localhost:6107>, or go straight to a culture with `?lang=de-DE`, `?lang=ar-SA` or
`?lang=qps-ploc` in a **new tab**.

## The five-minute demo

| Step | What to show |
|---|---|
| 1. Browser culture → **Deutsch** | `Offene Tickets diese Woche`, `23.09.2026`, `4.820,00 €`, `87,5 %`, `Offen` / `Wartend` / `Geschlossen`, and `Kundendatensatz speichern` on a button that grew to fit. Captions and formatted values change together. |
| 2. Press **Save customer record** | A Wisej.NET dialog. Title and message are ours; **Ja** / **Nein** are the framework's own German. |
| 3. Missing translation | Delete `TicketStatus.Closed` from `Strings.resx`, rebuild, and the third row reads `[TicketStatus.Closed]`. Put it back. |
| 4. **العربية** | The rail, the grid columns and the buttons mirror. `GD-1042` keeps reading left to right. |
| 5. **Pseudo (qps-ploc)** | Every caption bracketed and longer. Company names and ticket references are not, because they are data. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `LocalizationService` with `Text`, `Date`, `Number`, `Currency` | [LocalizationService.cs](GlobalDesk/LocalizationService.cs) |
| A single missing-key switch | `DevelopmentMode` — bracketed marker while developing, the key plus a log entry in production |
| The only place that touches `ResourceManager` | the same file; [Texts.cs](GlobalDesk/Texts.cs) is a forwarder kept for the earlier modules' call sites |
| Domain returns values, UI maps them to keys | [Ticket.cs](GlobalDesk/Ticket.cs) — `TicketStatus` plus `ResourceKeyFor` |
| Pseudo-localized resource set | [Strings.qps-ploc.resx](GlobalDesk/Resources/Strings.qps-ploc.resx), generated from the neutral values |
| The QA checklist, run | [docs/LocalizationQA.md](GlobalDesk/docs/LocalizationQA.md) |

## Notes

- `LocalizationService` reads `Application.CurrentCulture` **at call time**. Nothing caches a
  `CultureInfo`, a `NumberFormatInfo` or a formatted string, because all three are per session.
- `Currency` formats money; it does **not** convert it. An amount in euros shown to a US session
  renders as `$4,820.00` — the right shape and the wrong currency. A multi-currency application
  carries the currency in the data.
- The domain hands the UI a `TicketStatus`, never a caption. The mapping to a resource key lives
  beside the enum, so renaming a member is a compile error rather than a missing key at run time.
- `CultureChanged` repaints the captions, the tiles, the grid (whose cells hold formatted text)
  and rebuilds the designer-localized control, in one handler. The screen is never half translated.
- Publish and look in the output folder: `de`, `it`, `ar` and `qps-ploc` each need their satellite
  assembly beside the application. A packaging step that drops them falls back to English in
  silence.
