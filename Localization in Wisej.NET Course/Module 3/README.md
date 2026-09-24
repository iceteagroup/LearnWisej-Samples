# GlobalDesk · Localization in Wisej.NET · Module 3

Local lab build for **Module 3 · Application Resources, ResourceManager and Wisej System Text**.
The GlobalDesk customer editor exactly as the walkthrough shows it: the blue application bar, the
**Name** / **Email** / **Notes** rows, the italic last-order sentence, the green confirmation pill
and the **Save** / **Cancel** pair — plus the Wisej.NET `MessageBox` whose buttons are the
framework's own text.

The designed captions still come from `CustomerEditor.resx` (Module 2). What Module 3 adds is
everything the designer can never reach: `Resources/Strings.de.resx` beside the neutral file, a
`Customer.LastOrder` sentence with two placeholders built through `string.Format`, and a save
service that returns a `SaveResult` value the UI turns into words.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 3/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6103
```

Open <http://localhost:6103>, and <http://localhost:6103/?lang=de-DE> in a **new tab** for the
German session. `culture` is `auto` in `Default.json`, so each session starts from the browser's
`Accept-Language` and `?lang=` overrides it for that session only — reloading the same tab
reconnects to the session you already have.

## What to try

| Action | Expected result |
|---|---|
| Read the italic line | `Anna Weber last ordered on 9/14/2026.` — one resource string with `{0}` and `{1}`, not three fragments. |
| Open `?lang=de-DE` in a new tab | `Anna Weber hat zuletzt am 14.09.2026 bestellt.` The date moved into the middle of the sentence and the verb went to the end. |
| Press **Save** | The green pill, then a Wisej.NET `MessageBox`. |
| Press **Save** under German | `Kunde gespeichert.` from `Strings.de.resx`, and `OK` / `Abbrechen` on the dialog — Wisej.NET's own German, with no resource file of ours involved. |
| Clear **Name** and press **Save** | The field turns red and `This field is required.` appears beside it, from `Validation.Required`. |
| Type a broken address and press **Save** under German | `Enter a valid email address.` — in English. `Validation.Email` is missing from the German file on purpose: .NET falls back to the neutral value and nothing throws. |
| Two tabs, two languages, at once | One server process, two sessions, two cultures. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `Strings.de.resx` with the same keys | [Resources/Strings.de.resx](GlobalDesk/Resources/Strings.de.resx) — 22 of the neutral file's 23 |
| `Customer.LastOrder` with two placeholders and a translator comment | [Resources/Strings.resx](GlobalDesk/Resources/Strings.resx) |
| Built with `string.Format` and the session culture | `ShowLastOrder` in [CustomerEditor.cs](GlobalDesk/CustomerEditor.cs) |
| Save service that returns a result code, not a sentence | [CustomerSaveService.cs](GlobalDesk/CustomerSaveService.cs) |
| Every message routed through `Texts.Get` | `btnSave_Click` in [CustomerEditor.cs](GlobalDesk/CustomerEditor.cs) |
| `Resources.de.resx` overriding a Wisej system label | [Resources.de.resx](GlobalDesk/Resources.de.resx) — **and it does not take effect**, see the lab note |
| Lab note on product text, system text and diagnostic text | [docs/TextKinds.md](GlobalDesk/docs/TextKinds.md) |

## Notes

- `Texts.Get` is unchanged from Module 1: one `Wisej.Resources.ResourceManager` over the base name
  `GlobalDesk.Resources.Strings`, and a bracketed `[key]` when a lookup comes back empty.
- A missing **translation** and a missing **key** are different failures. The first falls back to
  the neutral value silently — `Validation.Email` in this build proves it. The second shows the
  bracketed marker. Only the second is visible without looking for it.
- The culture goes to `string.Format` as well as to each `ToString`. Without it, `string.Format`
  uses the thread's culture, which in a server application is whatever that thread last did.
- The `Resources.de.resx` override of Wisej's own `yes` / `no` labels **does not work here**, and
  the lab note says exactly what was tried and what was verified. Wisej already ships German for
  those buttons, which is why they are German anyway.
- Module 2's culture chip is gone from this screen: the walkthrough switches this module through
  the address bar instead. Module 4 puts a real language picker on the dashboard.
