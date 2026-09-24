# Module 6 lab note - one honest translation round

Italian went from nothing to complete in this module. What follows is what the round actually
consisted of and what it cost, because the tooling comparison only means something once you have
done the work once.

## What "no untranslated key left" means

`Strings.it.resx` has **29** entries against the neutral file's **32**. That is not a gap - it is
the right number, and the difference is the point of the next section.

## Invariant markers - three keys a translator never sees

Three entries in the neutral file are marked invariant, which ResX Resource Manager stores as
metadata beside the data:

```xml
<metadata name="App.Title.IsInvariant" xml:space="preserve">
  <value>True</value>
</metadata>
```

| Key | Value | Why it must not be translated |
|---|---|---|
| `App.Title` | `GlobalDesk` | Product name. Not translated, not declined, not transliterated. |
| `Product.Framework` | `Wisej.NET` | Third-party product name. Same rule, and it is not ours to change. |
| `Format.CustomerCode` | `AA0000` | The literal code format, quoted in help text. Translating the letters makes the help wrong. |

A key marked invariant disappears from the translation grid and no language file gets a value for
it, which is exactly what you want: the translator is not asked, so the translator cannot get it
wrong, and nobody has to review thirty language files to check that the product name survived.

The failure this prevents is real and expensive. Product names get "helpfully" translated all the
time - and a support engineer searching a customer's screenshot for "GlobalDesk" will not find
"Bureau Mondial".

## Comments - context for the keys that cannot be guessed

Seven keys carry a `<comment>`. Three of them are the ones that would otherwise be translated
wrongly, and the one-word `Open`-style caption the lab warns about has three siblings here:

| Key | Value | Why it is ambiguous without context |
|---|---|---|
| `Contacts.Role` | `Role` | A job function, a theatrical part and a security role are three different words in most languages. The comment says "job function such as Purchasing or Finance". |
| `Dashboard.LastOrder` | `Last order` | Noun phrase (the most recent order) or imperative (order it last)? The comment says which, and that it is a button. |
| `CustomerEditor.Save` | `Save` | Imperative verb or noun? Italian wants `Salva`, not `Salvataggio`. The comment says "imperative verb - the action". |
| `Dashboard.Fallback` | `Falls back to` | A sentence fragment completed at run time by a culture name. Without the comment a translator produces something that does not join up. |
| `Customer.LastOrder` | `Last order: {0} for {1}` | Says what each placeholder is and that word order may change. |

The rule worth carrying away: **a translator sees a spreadsheet, not your screen.** Anything whose
part of speech, length constraint or surrounding sentence is not obvious from the value alone needs
a comment. Writing them takes ten minutes and saves a review cycle per language.

## Export, translate, re-import

`docs/translation/Strings.export.csv` is the artefact that goes out and comes back: one row per
key, the comment, the invariant flag, then one column per language.

```
Key,Comment,Invariant,en,de,it,ar
"App.Title","Product name. Invariant - never translated...",X,"GlobalDesk","","",""
"Dashboard.Welcome","",,"Welcome to GlobalDesk","Willkommen bei GlobalDesk","Benvenuto in GlobalDesk","مرحبا بك..."
```

Three things about that file matter more than the format:

1. **The comment travels with the string.** A translator who has to ask what `Role` means has
   already cost more than the comment did.
2. **The invariant column is visible.** The translator can see that the blank is deliberate.
3. **Every language is in one file.** A translator working on Italian can see the German, which is
   often the fastest way to resolve an ambiguity the comment missed.

Re-importing produces a `.resx` diff, and the diff is the review. Things worth stopping on:

- **Placeholders.** `{0}` and `{1}` present, in a sensible order, not turned into `{0 }` by a
  spreadsheet. This is the single most common breakage and it throws `FormatException` at run time,
  not at build time.
- **Entries that should not have changed.** A diff touching `App.Title` means the invariant flag
  was lost somewhere in the round trip.
- **Whitespace and trailing full stops.** `Cliente salvato.` keeps the stop; `Salva` does not gain
  one.
- **Encoding.** The Arabic and the Italian accented characters have to survive. A CSV opened and
  re-saved by the wrong spreadsheet is where this goes wrong, silently.

Commit the round as **one change**. A reviewer can then read "Italian translation round" as a
single diff instead of finding stray `.resx` edits scattered through six feature commits.

## The three ways of doing this, compared

| | Visual Studio's `.resx` editor | ResX Resource Manager | A cloud TMS |
|---|---|---|---|
| Cost | free, already installed | free VS extension | per-seat or per-word |
| Sees all languages at once | no - one file per tab | **yes, a grid** | yes |
| Shows untranslated keys | no | **yes, a filter** | yes |
| Comments | yes, one column | yes, in the grid | yes |
| Invariant marking | no | **yes** | usually |
| Non-developers can use it | no | no - it lives in Visual Studio | **yes** |
| Translation memory, glossary | no | no | **yes** |
| Review workflow | pull request | pull request | built in |
| Where the truth lives | the repository | the repository | **the platform** |

**Visual Studio's editor** is fine for adding a key while you are writing the code that uses it.
It is unusable as a translation tool: one language per tab, no way to see what is missing, and
finding the untranslated keys means reading two files side by side.

**ResX Resource Manager** is the right default for a developer-run translation round, and it is
what this module used. The grid, the untranslated filter and the invariant flag are exactly the
three things the job needs, the `.resx` files stay the source of truth, and every change is an
ordinary diff in an ordinary pull request. Its limit is who can use it: it is a Visual Studio
extension, so a translator who is not a developer cannot.

**A cloud TMS** is what you move to when translators are not developers - and that is the real
trigger, not the number of languages. It buys translation memory (the second product reuses the
first's strings), a glossary, and a review workflow that does not involve teaching a translator
git. What it costs is that the platform becomes the source of truth and the repository becomes a
copy; export and import become a scheduled job someone has to own, and "which version is right"
becomes a question you can now ask.

**For a team whose translators are not developers**, the honest recommendation is: start with ResX
Resource Manager while the developers are still doing the translating, and move to a TMS the first
time you hand a language to someone outside the team. Do not start with a TMS for two languages a
developer can handle - the integration is more work than the translation. Do not stay on `.resx`
round-trips once a professional translator is involved, because emailing spreadsheets is how
placeholders get broken and how the fourth language quietly falls a release behind.

Whichever you choose, keep the comments and the invariant markers in the `.resx` files. They are
the part that survives a change of tooling.
