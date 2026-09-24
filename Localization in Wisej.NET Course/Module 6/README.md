# GlobalDesk · Localization in Wisej.NET · Module 6

Local lab build for **Module 6 · Running a Translation Round**. Italian added as a complete
language, translator comments on the keys that cannot be guessed from their value, invariant
markers on the product names and the code format, and the export the round trip is actually made
of. `it-IT` joins the picker.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 6/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6106
```

Open <http://localhost:6106>, or go straight to <http://localhost:6106/?lang=it-IT>.

## What to try

| Action | Expected result |
|---|---|
| Pick **it-IT** | `Benvenuto in GlobalDesk`, `Data di scadenza`, `Totale ordine`, and Italian formatting: `domenica 15 marzo 2026`, `12.500`, `1.850,75 €`. |
| Look at the culture row | `it-IT - italiano (Italia) (Ripiega su it)` - the fallback chain, in Italian. |
| Count the entries | 32 neutral, 29 Italian. The three missing are invariant, and that is correct. |
| Open `Strings.resx` | Three `IsInvariant` metadata entries and seven `<comment>` elements. |
| Open `docs/translation/Strings.export.csv` | One row per key, the comment, the invariant flag, one column per language. This is what a translator receives. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `Strings.it.resx` with no untranslated key left | [Resources/Strings.it.resx](GlobalDesk/Resources/Strings.it.resx) |
| Comments on the ambiguous keys | the `<comment>` elements in [Resources/Strings.resx](GlobalDesk/Resources/Strings.resx) |
| Invariant markers on the product name and the identifiers | the `IsInvariant` metadata in the same file |
| Export and the reviewed round trip | [docs/translation/Strings.export.csv](GlobalDesk/docs/translation/Strings.export.csv) |
| Workflow note comparing the three tools | [docs/TranslationWorkflow.md](GlobalDesk/docs/TranslationWorkflow.md) |

## A note on the tooling

ResX Resource Manager is a Visual Studio extension and this sample was not produced by driving it.
What the module ships is what a completed round **looks like** - the Italian file, the comments,
the `IsInvariant` metadata in the format the extension writes, and the export artefact - so the
files can be read, diffed and compared against your own round. The workflow note describes the
extension's grid, untranslated filter and invariant flag as the three things that make it the right
default, and says plainly where a cloud platform takes over.

## Notes for anyone running the next language

- **Mark invariant before you export.** A key marked invariant never reaches the translator, so it
  cannot come back translated. Product names, third-party names and literal formats.
- **Comment anything whose part of speech is not obvious.** `Role`, `Save`, `Last order` and any
  sentence fragment. A translator sees a spreadsheet, not your screen.
- **The diff is the review.** Check placeholders first - `{0}` broken by a spreadsheet throws at
  run time, not at build time - then entries that should not have changed, then trailing
  punctuation and encoding.
- **Commit the round as one change**, so a reviewer reads one diff instead of finding `.resx` edits
  scattered through feature commits.
- Keep the comments and the invariant markers in the `.resx` files. They are the part that survives
  a change of tooling.
