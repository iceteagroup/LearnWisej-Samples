# GlobalDesk · Localization in Wisej.NET · Module 7

Local lab build for **Module 7 · The Capstone**. A `LocalizationService` that is the only place
the application touches a `ResourceManager` or a `CultureInfo`, with `Text`, `Date`, `Number` and
`Currency` and a documented missing-key policy. A `Ticket` domain model that exposes a
`TicketStatus` the UI maps to a localized caption. A pseudo-localized pass that found real
hard-coded strings in this build. And a QA matrix the application can run on itself.
This folder is the complete application at the end of the course.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 7/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6107
```

Open <http://localhost:6107>, or go straight to <http://localhost:6107/?lang=qps-ploc>.

## What to try

| Action | Expected result |
|---|---|
| Press **Run the QA matrix** under any culture | One line covering the fallback chain, the three formats, RTL, a probe key, the published culture folders and the missing keys collected this session. |
| Run it under `qps-ploc` | `Ticket.Waiting -> "Waiting on customer"` **and** `No missing keys this session`. Read both together - that is the lesson. |
| Look at the tickets grid under each culture | The status column is a localized caption; the date and the amount are formatted. The reference never changes - it is an identifier. |
| Switch culture with the grid on screen | The grid is rebuilt. Its cells hold formatted text, so it cannot be left alone. |
| Set `LocalizationService.DevelopmentMode = false` | A missing key renders as the bare key instead of `[Key]`, and is still recorded. |
| Check the output folder | `ar`, `de`, `it`, `qps-ploc`. English is the neutral file and correctly has no folder. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `LocalizationService` with `Text`, `Date`, `Number`, `Currency` and a missing-key policy | [LocalizationService.cs](GlobalDesk/LocalizationService.cs) |
| Domain returning `TicketStatus`, UI mapping it to a caption | [Ticket.cs](GlobalDesk/Ticket.cs), `LoadTickets` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs) |
| Pseudo-localized set used to find hard-coded strings | [Resources/Strings.qps-ploc.resx](GlobalDesk/Resources/Strings.qps-ploc.resx) |
| The completed test matrix | `btnQa_Click` and [docs/LocalizationQA.md](GlobalDesk/docs/LocalizationQA.md) |
| Published output checked for culture folders | `DescribeSatellites` |
| The QA checklist | [docs/LocalizationQA.md](GlobalDesk/docs/LocalizationQA.md) |

## The finding this module is built around

`Ticket.Waiting` is deliberately absent from the pseudo-locale. The QA run reports the caption in
plain English **and** says "No missing keys this session" - both true at once. `GetString` walked
the fallback chain, found the neutral value and returned it, so the policy never fired.

**A missing-key policy and pseudo-localization catch different defects.** The policy catches a key
the code asks for that nobody wrote. Pseudo-localization catches a key that exists in the neutral
file but is missing from a translation, because that falls back silently and looks deliberate.
Neither sees the other's defect, so run both.

The same pass found two genuinely hard-coded column headers in this build, `Reference` and
`Status`, unbracketed beside a screen of bracketed captions. They now have keys in all five files.

## Notes for anyone extending the service

- Everything reads `Application.CurrentCulture` per call. No static state, ever - Module 4's note
  has the two-user failure in full.
- `Currency` formats; it does **not** convert. An amount in euros shown to a US session renders as
  `$1,850.75` - right shape, wrong currency. Multi-currency means the currency is part of the data.
- Pass the culture to `string.Format` as well as to each `ToString`. Without it, `string.Format`
  uses the thread's culture.
- `Ticket.ResourceKeyFor` is a `switch`, not `"Ticket." + status`. Concatenation compiles whatever
  you rename the enum to and fails at run time; the switch fails at the point of the change.
- The tickets grid is not data-bound to the model on purpose. Binding would show the enum name and
  the invariant `ToString` of the date, which is the defect this module is about.
- `<NeutralLanguage>en</NeutralLanguage>` is what declares that English needs no satellite. Without
  it the build produces an `en` folder nobody uses.
