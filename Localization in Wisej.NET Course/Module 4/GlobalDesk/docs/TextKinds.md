# Module 3 lab note - three kinds of text, three different owners

Press **Show a Wisej.NET dialog** under German and look at the box carefully. Four pieces of text
are on screen and they come from three different places.

| On screen | Comes from | Who owns it | Follows the culture switch? |
|---|---|---|---|
| Title `GlobalDesk` | `Strings.de.resx`, key `App.Title` | us | yes |
| Message `Kunde gespeichert.` | `Strings.de.resx`, key `CustomerEditor.Saved` | us | yes |
| Buttons `Ja` / `Nein` | Wisej.NET's `Wisej.Resources`, keys `yes` / `no` | the framework | yes, and without us doing anything |
| `SaveResult.Saved` in the log | the enum name | nobody translates it | no, deliberately |

## 1. Product text - ours

Everything the application says. It lives in `Resources/Strings.resx` with a translation per
language, and it reaches the screen through `Texts.Get`.

Two things Module 3 adds to it.

**A sentence with placeholders.** `Customer.LastOrder` is one resource string, not three fragments:

```
en: Last order: {0} for {1}
de: Letzte Bestellung: {0} über {1}
```

built with `string.Format` and the session culture:

```csharp
string.Format(culture, Texts.Get("Customer.LastOrder"),
              date.ToString("d", culture), amount.ToString("C", culture));
```

Concatenating `Texts.Get("LastOrderPrefix") + date + " for " + amount` would compile and would be
untranslatable: word order is not universal, the preposition depends on what follows it, and a
translator handed three fragments has no way to see the sentence. The placeholders also carry a
`<comment>` in the `.resx` saying what `{0}` and `{1}` are, which is the only context a translator
gets.

Note the culture is passed to `string.Format` as well as to each `ToString`. Without it,
`string.Format` uses the thread's culture, which in a server application is whatever the thread
last happened to be doing.

**A service that does not speak English.** `CustomerSaveService.Save` returns a `SaveResult` enum:

```csharp
var result = CustomerSaveService.Save(this.txtName.Text, this.txtCode.Text);
this.lblMessage.Text = Texts.Get(CustomerSaveService.ResourceKeyFor(result));
```

A service that returns `"Customer saved."` has made a language decision for every caller - the web
UI, a background job's log, a unit test - and only one of them wanted it. The enum is testable
without a culture, and the mapping from result to resource key sits next to the enum so that adding
a result and forgetting its caption is a compiler complaint rather than a blank message.

## 2. Wisej.NET system text - the framework's

The `Ja` and `Nein` buttons were never ours. They are keys `yes` and `no` in Wisej's own
`Wisej.Resources`, and the framework ships satellite assemblies for de, es, fr, it, ja, ko, pl, pt,
pt-BR, ru, tr, cs, zh-Hans and zh-Hant. Switch the session to German and they are German, with no
resource file in this project at all. The same goes for the file dialog, the colour dialog and the
session-timeout form - all of them have their own `.resources` in those satellites.

That is worth knowing on its own: **before translating a framework string, check whether it is
already translated.** For fourteen languages, most of this work is done.

### The override, and what actually happened

The module asks for an application-level `Resources-[LANG].resx` that overrides one of those
labels. This project contains `Resources.de.resx` attempting exactly that, with `yes` →
"Ja, speichern" and `no` → "Nein, verwerfen".

**It does not take effect.** Under German the buttons still read `Ja` and `Nein`. Two namings were
tried and both were verified by reading the built assembly:

| File | Where it ended up | Effect |
|---|---|---|
| `Resources-de.resx` | `GlobalDesk.Resources-de.resources`, in the **main** assembly - the hyphen is not a culture, so MSBuild does not build a satellite | none |
| `Resources.de.resx` | `GlobalDesk.Resources.de.resources`, correctly in the `de` satellite | none |

The second one is at least the right *shape* - it is a proper culture-specific resource in the
right satellite - so the remaining unknown is the base name Wisej.NET looks for. Wisej's own
strings live under base name `Wisej.Resources`; an application resource gets its root namespace
prefixed, so this project's is `GlobalDesk.Resources`, and those do not match.

If you need this working, the things to check, in order:

1. Whether Wisej.NET expects a specific manifest name rather than whatever the root namespace
   produces. Force one with
   `<EmbeddedResource Include="Resources.de.resx" LogicalName="..." />` and try the candidates.
2. Whether the override is meant to be registered in code rather than discovered - `Wisej.Resources.ResourceManager`
   is public and takes `(baseName, assembly)`.
3. `Default.json`, which is where several Wisej.NET behaviours are switched on.

The file is left in the project with its comments intact, and the status line in the application
reports what is really on the buttons rather than what was intended. A sample that silently claims
a feature works is worse than one that says it could not get it working.

## 3. Diagnostic text - nobody's

`SaveResult.Saved`, log messages, exception type names, the customer code format `AA0000`, database
column names. None of it is translated, on purpose. A log a support engineer has to read in a
language they do not speak is worse than useless, and an identifier that changes with the reader's
language is not an identifier.

The rule: **if a machine or an engineer reads it, leave it alone. If a user reads it, translate
it.** `Validation.CodeFormat` is translated because a user reads it; the pattern `^[A-Za-z]{2}[0-9]{4}$`
is not, because it is the rule itself.
