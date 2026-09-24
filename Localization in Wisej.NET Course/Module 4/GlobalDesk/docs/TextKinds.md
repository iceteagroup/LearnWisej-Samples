# Module 3 lab note - three kinds of text, three different owners

Press **Save** under German and look at what is on screen. Five pieces of text, from three
different places.

| On screen | Comes from | Who owns it | Follows the culture switch? |
|---|---|---|---|
| Field labels `Name` / `E-Mail` / `Notizen` | `CustomerEditor.de.resx` | us, through the designer | only on a new instance |
| `Anna Weber hat zuletzt am 14.09.2026 bestellt.` | `Strings.de.resx`, key `Customer.LastOrder` | us | yes, on the next lookup |
| `Kunde gespeichert.` | `Strings.de.resx`, key `CustomerEditor.Saved` | us | yes |
| Dialog buttons `OK` / `Abbrechen` | Wisej.NET's own `Wisej.Resources` | the framework | yes, and without us doing anything |
| `SaveResult.Saved` in a log | the enum name | nobody translates it | no, deliberately |

## 1. Product text - ours

Everything the application says. It lives in `Resources/Strings.resx` with a translation per
language, and it reaches the screen through `Texts.Get`.

Two things Module 3 adds to it.

**A sentence with placeholders.** `Customer.LastOrder` is one resource string, not three fragments:

```
en: {0} last ordered on {1}.
de: {0} hat zuletzt am {1} bestellt.
```

built with `string.Format` and the session culture:

```csharp
string.Format(culture, Texts.Get("Customer.LastOrder"),
              CustomerName, LastOrderDate.ToString("d", culture));
```

German moves the date into the middle of the sentence and the verb to the end. No amount of
`Texts.Get("LastOrderPrefix") + name + " last ordered on " + date` can produce that: word order is
not universal, and a translator handed three fragments cannot see the sentence they are
translating. The placeholders carry a `<comment>` in the `.resx` saying what `{0}` and `{1}` are,
which is the only context a translator gets.

Note the culture is passed to `string.Format` as well as to `ToString`. Without it,
`string.Format` uses the thread's culture, which in a server application is whatever the thread
last happened to be doing.

**A service that does not speak English.** `CustomerSaveService.Save` returns a `SaveResult`:

```csharp
var result  = CustomerSaveService.Save(this.txtName.Text, this.txtEmail.Text);
var message = Texts.Get(CustomerSaveService.ResourceKeyFor(result));
```

A service that returns `"Customer saved."` has made a language decision for every caller - the web
UI, a background job's log, a unit test - and only one of them wanted it. The enum is testable
without a culture, and the mapping from result to resource key sits next to the enum, so adding a
result and forgetting its caption is a compiler complaint rather than a blank message.

## 2. Wisej.NET system text - the framework's

The `Abbrechen` on the dialog was never ours. Wisej.NET's own strings live in `Wisej.Resources`,
and the framework ships satellite assemblies for de, es, fr, it, ja, ko, pl, pt, pt-BR, ru, tr, cs,
zh-Hans and zh-Hant. Switch the session to German and the dialog is German, with no resource file
in this project involved at all. The same goes for the file dialog, the colour dialog and the
session-timeout form.

That is worth knowing on its own: **before translating a framework string, check whether it is
already translated.** For fourteen languages, most of this work is done.

### The override, and what actually happened

The module asks for an application-level resource that overrides one of those labels. This project
contains `Resources.de.resx` attempting exactly that, with `yes` → "Ja, speichern" and `no` →
"Nein, verwerfen".

**It does not take effect.** Under German the dialog still shows Wisej's own labels. Two namings
were tried and both were verified by reading the built assembly:

| File | Where it ended up | Effect |
|---|---|---|
| `Resources-de.resx` | `GlobalDesk.Resources-de.resources`, in the **main** assembly - the hyphen is not a culture, so MSBuild does not build a satellite | none |
| `Resources.de.resx` | `GlobalDesk.Resources.de.resources`, correctly in the `de` satellite | none |

The second is at least the right *shape* - a proper culture-specific resource in the right
satellite - so the remaining unknown is the base name Wisej.NET looks for. Wisej's own strings sit
under base name `Wisej.Resources`; an application resource gets its root namespace prefixed, so
this project's is `GlobalDesk.Resources`, and the two do not meet.

If you need this working, the things to check, in order:

1. Whether Wisej.NET expects a specific manifest name rather than whatever the root namespace
   produces. Force one with
   `<EmbeddedResource Include="Resources.de.resx" LogicalName="..." />` and try the candidates.
2. Whether the override is meant to be registered in code rather than discovered -
   `Wisej.Resources.ResourceManager` is public and takes `(baseName, assembly)`.
3. `Default.json`, which is where several Wisej.NET behaviours are switched on.

The file is left in the project with its comments intact. A sample that silently claims a feature
works is worse than one that says it could not get it working.

## 3. Diagnostic text - nobody's

`SaveResult.Saved`, log messages, exception type names, the email pattern
`^[^@\s]+@[^@\s]+\.[^@\s]+$`, database column names. None of it is translated, on purpose. A log a
support engineer has to read in a language they do not speak is worse than useless, and an
identifier that changes with the reader's language is not an identifier.

The rule: **if a machine or an engineer reads it, leave it alone. If a user reads it, translate
it.** `Validation.Email` is translated because a user reads it; the pattern is not, because it is
the rule itself.

## The two ways a translation goes missing

`Validation.Email` is deliberately absent from `Strings.de.resx`. Break the address under German
and the message appears - in English. Nothing threw, nothing logged, and the screen still looks
finished. That is what a missing *translation* does: .NET walks `de-DE`, then `de`, then the
neutral file, and stops at the first value it finds.

A missing *key* is the opposite: `Texts.Get` returns `[Validation.Email]`, which nobody can mistake
for finished text.

**Both are real, and neither test finds the other.** A missing-key marker catches a key nobody ever
wrote; only a side-by-side review, or Module 5's pseudo-localized pass, catches a key that exists
but was never translated.
