# Module 6 lab note - one translation round, and how to run the next one

Adding Italian to GlobalDesk was not a translation task. It was a process with a number attached
to it, and the number is what made it finishable.

## The grid, not the XML

Every key down the rows, every language across the columns, one screen. Whether that is ResX
Resource Manager, the Visual Studio editor or a cloud platform matters less than the shape: a
translator has to see **all** the languages for a key at once, and a developer has to see which
cells are empty without opening six files.

Editing the `.resx` by hand is where rounds go wrong. The XML is a serialization format; the grid
is the interface.

## The measure

**Untranslated count.** Before the round, four keys had no Italian value. Filter, translate,
filter again, and the number is zero. Anything else is an opinion.

The number is the deliverable, not the feeling that Italian "looks done".

## What the comments are for

A translator sees a key, a value and a comment. That is all the context there is.

| Key | Value | Why it needs a comment |
|---|---|---|
| `CustomerEditor.Open` | Open | The **verb** - open the selected customer. |
| `Ticket.Open` | Open | The **state** - the ticket is open. |
| `Customer.LastOrder` | Last order: {0} on {1} | `{0}` is the order number, `{1}` the date. Nothing in the string says so. |
| `Rail.Culture` | Culture | The .NET sense - language plus regional formatting - not culture as in the arts. |
| `Dashboard.WelcomeBack` | Welcome back, {0}. | Italian inflects the greeting for the person's gender: *Bentornata* for Ana, *Bentornato* for a man. |

The two `Open` keys are the whole argument for semantic keys in one line. As `"Open"` they are the
same string and a tool would offer to merge them; as two keys with two comments the Italian comes
back as **Apri** and **Aperto**, which are not interchangeable.

## What the invariant markers are for

`App.ProductName`, `Product.Framework`, `Format.CustomerCode` and the six `Language.*` entries
carry an `IsInvariant` marker in the neutral file:

```xml
<metadata name="App.ProductName.IsInvariant" xml:space="preserve">
  <value>True</value>
</metadata>
```

A marked key never reaches a translator, so it cannot come back translated. That is not a
theoretical risk: a machine draft will happily render **GlobalDesk** as *Scrivania globale* and
**Wisej.NET** as *Wisej.RETE*, and a reviewer reading 200 rows will not catch it.

The language list is invariant for a different reason. A language list shows every language in
**its own** language - a German speaker looking for German looks for *Deutsch* - so those values
are the same in every file on purpose.

## What the review caught

The machine draft of `Customer.LastOrder` came back as:

```
Ultimo ordine: {1} il {0}
```

The placeholders are both present, the sentence reads correctly in Italian, and it compiles. It
would have printed the order **date** where the number belongs and the number where the date
belongs, in production, in one language, on one screen.

Placeholders are the first thing to check in a diff, every time:

- both present,
- not renumbered,
- in an order the sentence actually needs.

## Read the diff like code

The tool saves as you type. That makes `git diff Strings.it.resx` the review, and the round should
be one commit with the values, the comments and the invariant markers together - not a commit per
save.

Things that show up in a diff and nowhere else: a key quietly renamed, a value with a trailing
space, an entity that got double-escaped on a round trip, a comment deleted because the tool
thought the row was empty.

## Finish the round on the screen, not in the grid

`Texts.UntranslatedKeys()` asks the running application the same question the filter asks the
grid: which keys still come back with the neutral value? The rail reports the count, and the
tooltip names them.

It exists because fallback is silent. A key missing from `Strings.it.resx` renders in English and
the Italian screen still looks complete - the grid is the only place it looks wrong, and only if
somebody opens the grid.

**The report is a heuristic, not a verdict.** Italian legitimately writes **Email**, and *Ana* is
*Ana* in every language, so both are counted as "same as neutral" while being perfectly correct.
That is why the label is amber rather than red: it exists to make a reviewer look at a short list,
not to fail a build.

The two reports this course has built see different failures and neither sees the other's:

| Report | Finds | Misses |
|---|---|---|
| Missing-key marker `[Key]` | a key nobody ever wrote | a key that exists and was never translated |
| Untranslated / same-as-neutral | a key that fell back to English | a key nobody ever wrote |
| Pseudo-localized pass | a string that never went through a resource at all | both of the above |

Run all three.

## Choosing a tool

| | Visual Studio `.resx` editor | ResX Resource Manager | Cloud TMS |
|---|---|---|---|
| Languages on screen at once | one | all | all |
| Untranslated filter | no | yes | yes |
| Comments visible while translating | yes | yes | yes |
| Needs Visual Studio | yes | extension or standalone | no |
| Translator can work without a developer | no | with the standalone build | yes |
| Review as a code diff | yes | yes | needs an export step |
| Translation memory, glossary, vendors | no | no | yes |

For a team whose translators are not developers, the deciding line is the second-to-last one. A
cloud platform is where translators can work on their own; the cost is that the review stops being
a `git diff` unless you export the round back into the repository and read it there.

For this project - six languages, one developer doing the imports - ResX Resource Manager is
enough, and the diff stays where the rest of the review already happens.
