# GlobalDesk · Localization in Wisej.NET · Module 3

Local lab build for **Module 3 · Shared Resources and Translated Text**. `Strings.de.resx` beside
the neutral file with the same keys translated, a `Customer.LastOrder` sentence built from one
resource string with two placeholders, a save service that returns a result code rather than an
English sentence, and a Wisej.NET dialog that puts product text and framework text side by side.
The Module 1-2 dashboard and editor are unchanged.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 3/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6103
```

Open <http://localhost:6103>.

## What to try

| Action | Expected result |
|---|---|
| Switch to **de-DE** | Every caption on the page turns German now - `Strings.de.resx` exists. The editor still does not; press Recreate. |
| Press **Letzte Bestellung** | One sentence from one resource string: `Letzte Bestellung: 15.03.2026 über 1.850,75 €`. Switch back to English and it reads `Last order: 3/15/2026 for $1,850.75`. |
| Press **Save** with an empty name | `Dieses Feld ist erforderlich.` - and the service returned `SaveResult.NameRequired`, not a German sentence. |
| Type `XX99` as the code and Save | The code-format message, from the same route. |
| Press **Einen Wisej.NET-Dialog anzeigen** | Title and message German from our resource; buttons `Ja` / `Nein` German from Wisej.NET's own, with no work from us. |
| Read the status line after that dialog | It says the project's `Resources.de.resx` override did **not** replace those buttons. That is the honest result - see the note. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `Strings.de.resx` with the same keys translated | [Resources/Strings.de.resx](GlobalDesk/Resources/Strings.de.resx) |
| `Customer.LastOrder` built with `string.Format` and the active culture | `ShowLastOrder` in [CustomerEditor.cs](GlobalDesk/CustomerEditor.cs) |
| A save service returning a result code, not a sentence | [CustomerSaveService.cs](GlobalDesk/CustomerSaveService.cs) |
| An application resource overriding a Wisej system label, shown in a MessageBox | [Resources.de.resx](GlobalDesk/Resources.de.resx) and `btnSystemText_Click` — **attempted, does not take effect**, see the note |
| Lab note separating product, system and diagnostic text | [docs/TextKinds.md](GlobalDesk/docs/TextKinds.md) |

## Known gap

The Wisej system-label override does not work in this sample. Two namings were tried and both were
verified against the built assembly: `Resources-de.resx` lands in the main assembly because the
hyphen is not a culture, and `Resources.de.resx` lands correctly in the `de` satellite but Wisej
still uses its own captions. The file, the attempt and the things left to check are documented in
[docs/TextKinds.md](GlobalDesk/docs/TextKinds.md), and the application reports what is really on
the buttons rather than claiming a success it did not have.

Everything else in the module works and is verified.

## Notes for anyone extending the resources

- **Wisej.NET already ships translations for its own strings** - de, es, fr, it, ja, ko, pl, pt,
  pt-BR, ru, tr, cs, zh-Hans, zh-Hant. Check before translating a framework label.
- A `.resx` whose culture is separated by a **hyphen** is not a culture-specific resource. MSBuild
  only builds a satellite for `Name.<culture>.resx` with a dot.
- One resource string per sentence, with placeholders. Never concatenate fragments - word order is
  not universal, and a translator needs the whole sentence.
- Pass the culture to `string.Format` as well as to each `ToString`. Without it `string.Format`
  uses the thread's culture.
- Put a `<comment>` on any key whose meaning is not obvious out of context, especially ones with
  placeholders. It is the only context a translator gets.
- Domain code returns codes; the UI turns codes into words. `CustomerSaveService` never mentions a
  language and can be tested without one.
