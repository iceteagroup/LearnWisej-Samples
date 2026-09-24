# Module 7 capstone - the localization QA checklist, and what running it found

Every row below was run against this build at `http://localhost:6107`. Where a row records a
defect, the defect and its fix are both named.

## The checklist

| # | Case | How to run it | Result |
|---|---|---|---|
| 1 | Neutral culture | `?lang=en-US` | Pass. `9/23/2026`, `$4,820.00`, `87.5%`, statuses `Open` / `Waiting` / `Closed`. |
| 2 | A translated language | `?lang=de-DE` | Pass. `23.09.2026`, `4.820,00 €`, `87,5 %`, `Offen` / `Wartend` / `Geschlossen`, and `Kundendatensatz speichern` on an `AutoSize` button that grew to fit. |
| 3 | A language-region culture with no file of its own | `?lang=fr-CA` | Pass, and this is the row that teaches the most: **English text, French-Canadian formatting**. `23 septembre 2026`, `4 820,00 $`, `87,5 %`. Text falls back `fr-CA` → `fr` → neutral; formatting never consults a resource file at all. |
| 4 | A deliberately missing key | Delete `TicketStatus.Closed` from `Strings.resx`, rebuild | Pass. The third row's status reads `[TicketStatus.Closed]`, the page renders, nothing throws, and `LocalizationService.MissingKeysThisSession` names the key. Restore it afterwards. |
| 5 | A date | any culture | Pass. `LocalizationService.Date` — `ToString("d", culture)`, never a hand-written pattern. |
| 6 | A number | any culture | Pass. `Number(value, "P1")` gives `87.5%` and `87,5 %` — note German's space before the sign, which nobody would have typed by hand. |
| 7 | A currency amount | any culture | Pass with a caveat. `$4,820.00` and `4.820,00 €` are the same **number**, formatted twice. The service formats money; it does not convert it. |
| 8 | A runtime switch with a designer-localized control on screen | pick a language from the rail | Pass. `CultureChanged` reapplies the captions, reformats the tiles, rebuilds the grid **and** rebuilds `CustomerEditor`, so the window is never half translated. |
| 9 | Long strings | `?lang=qps-ploc` | Pass. Every caption bracketed and about 40% longer; nothing clips. The ticket references, company names and formatted values are **not** bracketed, which is correct - they are data. |
| 10 | An RTL culture | `?lang=ar-SA` | Pass. Rail, grid columns and buttons mirror. `GD-1042` still reads left to right. **And the dates are not what you expect** - see below. |
| 11 | A Wisej.NET system label | press **Save customer record** | Pass. The title and the message are ours; the **Yes** / **No** buttons are Wisej's own, German under `de-DE`, with no resource file of ours involved. |
| 12 | The satellite assemblies survive a publish | `dotnet publish`, then look in the output | Pass. `de`, `it`, `ar` and `qps-ploc` each have a folder with `GlobalDesk.resources.dll`. |

## Row 10, in more detail: the calendar is part of the culture

Under `ar-SA` the due date does not read `23/09/2026`. It reads **`12/4/1448 بعد الهجرة`**, and
that is correct: `ar-SA`'s default calendar is **Umm al-Qura**, not the Gregorian one.
`CultureInfo.DateTimeFormat.Calendar` decides it, the same `DateTime` is being formatted, and
nobody wrote a line of code about calendars.

The digits move too. `ar-SA` uses the Arabic decimal and group separators, so the amount is
`4٫820٫00 ر.س.` rather than `4,820.00 SAR`.

Three things follow from that, and all three are worth writing down before a customer finds them:

- **Never store a formatted date.** A Hijri string cannot be parsed back with
  `DateTime.Parse(value)` under another culture, and it cannot be sorted. Store the `DateTime`.
- **A date that must be Gregorian for everybody** - a contract date, an audit timestamp - has to
  say so: format it with an invariant culture, or with a culture whose calendar you have chosen on
  purpose. `ar-EG` is Arabic with a Gregorian calendar, which is why some products offer both.
- **Test with the culture you will ship**, not with a culture that happens to be easier. `ar-EG`
  and `ar-SA` are both Arabic, both right-to-left, and they do not render the same date.

## Row 12, in more detail

The packaging step that drops satellites is the one that copies "the exe and the dlls" from the
build output. The culture folders are directories, and a copy that is not recursive leaves them
behind. The application then starts, runs, and is in English for everybody - with no error, no
log entry and no failing test.

Check it explicitly, once, in whatever pipeline actually ships:

```
bin/Release/net10.0/publish/
  GlobalDesk.dll
  ar/GlobalDesk.resources.dll
  de/GlobalDesk.resources.dll
  it/GlobalDesk.resources.dll
  qps-ploc/GlobalDesk.resources.dll
```

## One door for every string

```csharp
LocalizationService.Text("Ticket.Status")
LocalizationService.Text("Customer.LastOrder", number, date)
LocalizationService.Date(ticket.Due)
LocalizationService.Number(share, "P1")
LocalizationService.Currency(ticket.Amount)
```

Six modules produced six habits - `Texts.Get`, `ToString("D", culture)`,
`string.Format(culture, …)` - and the capstone collapses them into one surface. The value is not
tidiness. It is that every localization decision now has exactly one place to be made and audited:

- **What a missing key does.** One flag, `DevelopmentMode`. A bracketed marker while developing so
  a tester can describe the gap; the bare key plus a log entry in production, because a user
  should not be shown brackets and a blank caption is worse - it reads as a design choice and
  nobody reports it. It must never return an empty string and never throw.
- **Which culture is used.** `Application.CurrentCulture`, read at the moment of the call. Nothing
  caches it. A cached `CultureInfo` in a static field is Module 4's bug: one user's language
  change repaints another user's screen, and it never reproduces with one developer and one
  browser.
- **Whether a lookup is recorded.** Both reports live here:
  `MissingKeysThisSession` and `UntranslatedKeys()`.

## Three reports, three different blind spots

| Report | Finds | Cannot see |
|---|---|---|
| `[Key]` marker | a key nobody ever wrote | a key that exists and was never translated |
| `UntranslatedKeys()` | a key that fell back to the neutral value | a key nobody ever wrote; and it flags values a translator legitimately left identical |
| Pseudo-localized pass | a string that never went through a resource at all | both of the above |

Run all three. The pseudo pass is the only one that catches a hard-coded literal, because a
literal never asks the service for anything.

## What the domain is not allowed to do

```csharp
public TicketStatus Status { get; }                 // yes
public string StatusText => "In progress";          // no
public string FormattedDueDate => Due.ToString();   // no
```

`Ticket` exposes a `TicketStatus`, a `DateTime` and a `decimal`. The UI maps the status to a
resource key through `Ticket.ResourceKeyFor`, a `switch` that sits beside the enum so renaming a
member is a compile error rather than a run-time missing key.

A model that hands out display text has decided a language for a report, a background job, an
export and a unit test - and only the screen wanted one.

## What stays untranslated, on purpose

Ticket references (`GD-1042`), company names, log messages, exception type names, enum member
names, the customer-code format `AA0000` and the culture names in the picker's own data. If a
machine or an engineer reads it, leave it alone; if a user reads it, translate it.

The picker's **captions** are the interesting edge: they are resource values, and they are the
same in every file, marked invariant. A language list shows every language in its own language -
a German speaker looking for German looks for *Deutsch*.
