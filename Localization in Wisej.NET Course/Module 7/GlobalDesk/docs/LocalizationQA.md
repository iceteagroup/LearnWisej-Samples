# GlobalDesk localization QA checklist

The capstone deliverable. Every row below was run against this build; the results are the ones
quoted.

## The architecture, in one paragraph

`LocalizationService` is the only place the application touches a `ResourceManager` or a
`CultureInfo`. It exposes `Text`, `Date`, `Number` and `Currency`, it reads
`Application.CurrentCulture` on every call, and it owns the missing-key policy. `Texts.Get` is
kept as a thin forwarder so six modules of call sites still compile - a shim to delete, not a
second API. The domain returns `TicketStatus`; `Ticket.ResourceKeyFor` maps it to a key; the UI
turns the key into a word. Nothing below the UI has ever heard of a language.

## The missing-key policy

| Mode | A missing key returns | Why |
|---|---|---|
| Development (`DevelopmentMode = true`) | `[Ticket.Waiting]` | Visible on screen, describable by a tester, and the page still renders. |
| Production | `Ticket.Waiting`, and the key is recorded | A user must never see square brackets, but a **blank** caption is worse - it reads as a design choice and nobody reports it. The key tells a support engineer what to search for; the log is what gets it fixed. |

It never returns an empty string and it never throws. A missing translation is a content defect,
not a crash, and one absent key must not take a page down.

## The test matrix

Run `?lang=` for each culture and press **Run the QA matrix**.

| # | Case | Culture | Result |
|---|---|---|---|
| 1 | Neutral culture | `en-US` | All text from `Strings.resx`. Baseline. |
| 2 | Full translation | `de-DE`, `it-IT` | Every caption translated; dates, numbers and currency in the local convention. |
| 3 | Language-region falling back | `de-AT` | Text from `Strings.de.resx`; formatting genuinely Austrian - `12 500` and `€ 1.850,75`, against German's `12.500` and `1.850,75 €`. |
| 4 | Formatted values | any | `date 9/24/2026, number 1,234,567, currency ¤1,850.75` under the pseudo-locale; the `¤` is the generic currency sign, which is correct for a culture with no currency of its own. |
| 5 | Runtime switch | any → any | One `CultureChanged` handler refreshes captions, reformats values, rebuilds the editor and re-renders the tickets grid. |
| 6 | Right-to-left | `ar-EG` | `RightToLeft True`. The screen mirrors; `txtCode` does not. See Module 5's note. |
| 7 | Long strings / hard-coded text | `qps-ploc` | Every resource-backed caption bracketed and lengthened. Anything unbracketed never went through a resource. |
| 8 | A deliberately missing key | `qps-ploc` | `Ticket.Waiting -> "Waiting on customer"` - see below. |
| 9 | Published culture folders | any | `Culture folders present: ar, de, it, qps-ploc`. |

## What row 8 actually proved

`Ticket.Waiting` is deliberately absent from `Strings.qps-ploc.resx`. The QA run reports:

```
Ticket.Waiting -> "Waiting on customer". ... No missing keys this session.
```

Read those two sentences together. The key **is** missing from the pseudo-locale, the caption
**did** come out in plain English beside a screen full of bracketed text - and the missing-key
policy reported **nothing**.

That is not a bug in the policy. `ResourceManager.GetString` walked the fallback chain, found the
value in the neutral file and returned it, so as far as `LocalizationService` is concerned nothing
was missing. The policy can only ever catch a key absent from the **neutral** file.

The conclusion is the one worth carrying into any project: **a missing-key policy and
pseudo-localization catch different defects and you need both.**

- The policy catches a key the *code* asks for that nobody ever wrote - a typo, a rename, a new
  feature whose resource was forgotten.
- Pseudo-localization catches a key that exists in the neutral file but is missing from a
  *translation*, because that falls back silently and looks deliberate.

Neither sees the other's defect. Run both.

## What the pseudo-locale pass found in this build

Two column headers in the tickets grid, `Reference` and `Status`, were hard-coded English. They
appeared unbracketed beside a screen full of `[Bracketed ~~~]` captions, which is precisely the
signal the tool exists to give. They now have `Ticket.Reference` and `Ticket.Status` keys in all
five files, and the comment in `LoadTickets` says how they were found.

It also re-confirmed the Module 5 finding: the `CustomerEditor` captions are unbracketed under
`qps-ploc`, because designer resources are a separate translation job and there is no
`CustomerEditor.qps-ploc.resx`. Shared text being complete does not make a screen finished.

## The published output

Satellite assemblies live in per-culture folders beside the application. A deployment step that
copies the executable and forgets the folders leaves every session silently falling back to the
neutral text - and nothing errors, which is why this check is in the matrix rather than in
somebody's memory:

```csharp
foreach (var directory in Directory.GetDirectories(root))
    if (File.Exists(Path.Combine(directory, "GlobalDesk.resources.dll")))
        found.Add(Path.GetFileName(directory));
```

Reported live as `Culture folders present: ar, de, it, qps-ploc`. Four folders for four
non-neutral languages; English is the neutral file and correctly has none, which
`<NeutralLanguage>en</NeutralLanguage>` in the `.csproj` is what declares.

Ship `qps-ploc` or not - that is a decision, not an accident. It costs one small folder and gives
support a way to reproduce a layout complaint.

## Before calling a screen localized

- [ ] No user-visible literal in any `.cs` or `.Designer.cs` file.
- [ ] Every caption assigned in a method that `CultureChanged` calls again.
- [ ] Designer-localized controls rebuilt on a culture change, and their own `.resx` per language.
- [ ] Values formatted through `LocalizationService`, never with a custom pattern.
- [ ] Sentences composed from one resource string with placeholders, never concatenated.
- [ ] Comments on every key whose part of speech or context is not obvious.
- [ ] Product names and literal formats marked invariant.
- [ ] `qps-ploc` pass: everything bracketed, nothing clipped.
- [ ] RTL pass: mirrored, with identifiers pinned LTR.
- [ ] Two browsers open, language changed in one, nothing moves in the other.
- [ ] Culture folders present in the published output.
