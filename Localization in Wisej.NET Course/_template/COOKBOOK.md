# GlobalDesk cookbook - Wisej-4 4.1.4 / .NET 10

Established by running the labs and reading the result back. Each entry says how.

## Structure

One project per module under `Module N/GlobalDesk`, namespace `GlobalDesk`, ports 6101-6107.
Shared text in `Resources/Strings*.resx`; designer-localized controls carry their own
`<Control>.<culture>.resx` beside them.

## The two jobs, kept apart

**Text** comes from a resource and changes with the language. **Values** are `DateTime`,
`decimal` and `int`, formatted at display time with the session's culture as the format provider.
Nothing is ever stored formatted.

```csharp
value.ToString("D",  culture)   // date   - not "dd/MM/yyyy"
value.ToString("N0", culture)   // number - not a hand-typed separator
value.ToString("C",  culture)   // money  - not "€ " + amount
```

The test that settles arguments: **if you stored it, could you still sort by it?**

## `Wisej.Resources.ResourceManager`

Constructed with `(baseName, assembly)`. The base name is the manifest name without the culture or
the extension: `GlobalDesk.Resources.Strings` for `Resources/Strings.resx` in a project whose root
namespace is `GlobalDesk`. It reads `Application.CurrentCulture` and walks .NET's fallback chain -
`fr-CA`, then `fr`, then neutral - so **the neutral file must be complete**; it is the last place
the search looks.

Wrap it once. A missing key should return the key, bracketed in development, never an empty string
and never an exception.

## Culture

- `Application.CurrentCulture` is **per session**. Never copy it into a `static` field: one user's
  click then changes another user's screen, and it never reproduces with one developer and one
  browser.
- `Application.CultureChanged` is the hook. Assign the culture in one place and put every refresh
  behind the event, so the URL parameter, a saved preference and a support tool all get it.
- **Unsubscribe in `Dispose`.** The event outlives the page.
- `"culture": "auto"` in `Default.json` starts each session from `Accept-Language`; `?lang=de-AT`
  overrides it for that session. A **fixed** culture is right when values must be unambiguous for
  every reader - an internal back office, or numbers compared across users.
- A language-region culture with no resource file of its own makes fallback visible. `de-AT` gives
  German text with Austrian formatting: `12 500` and `€ 1.850,75` against German's `12.500` and
  `1.850,75 €`.

## Designer localization

Setting `Localizable` moves every localizable property into `<Control>.resx` and the generated code
becomes `resources.ApplyResources(control, "name")`.

**`ApplyResources` runs inside `InitializeComponent`, so designer resources are applied once, at
construction.** A culture change does nothing to an existing instance. Rebuild it:

```csharp
host.Controls.Clear();
editor?.Dispose();
editor = new CustomerEditor();
host.Controls.Add(editor);
```

A host panel makes that three lines instead of a hunt. Dispose matters - the old instance has a
client counterpart. Rebuilding loses unsaved input, which is the real cost of this approach.

**Override only what must differ** in a language file. German here overrides nine entries and
inherits nineteen. Every entry copied without changing is a future divergence: change the neutral
layout and the language silently keeps the old geometry.

Widening one button moves the one beside it, so a `Size` override usually brings a `Location`
override with it. `AutoSize` with a `MinimumSize` avoids per-language geometry entirely.

**Designer text and shared text are two separate translation jobs.** A complete `Strings.ar.resx`
does not translate `CustomerEditor`; it falls back silently and the screen looks intentional.

## `.resx` mechanics

- A culture must be separated by a **dot**: `Strings.de.resx`. `Strings-de.resx` is not a
  culture-specific resource and MSBuild embeds it in the main assembly instead of a satellite.
  Verified by reading the built output.
- Satellites land in per-culture folders beside the application. A deployment that misses them
  falls back to neutral silently - worth an explicit check.
- `<NeutralLanguage>en</NeutralLanguage>` declares that English needs no satellite.
- Comments travel to the translator and are the only context they get. Put one on anything whose
  part of speech, length constraint or surrounding sentence is not obvious: `Role`, `Save`,
  `Last order`, any fragment, anything with placeholders.
- ResX Resource Manager stores an invariant flag as `<metadata name="Key.IsInvariant">True</...>`
  in the neutral file. Marked keys never reach the translator, so they cannot come back translated
  - product names, third-party names, literal formats.

## Composed sentences

One resource string per sentence, with placeholders, and the culture passed to `string.Format` as
well as to each `ToString`:

```csharp
string.Format(culture, Texts.Get("Customer.LastOrder"),
              date.ToString("d", culture), amount.ToString("C", culture));
```

Concatenating fragments is untranslatable: word order is not universal and a translator handed
three pieces cannot see the sentence.

## Domain code returns codes

A service that returns `"Customer saved."` has made a language decision for every caller. Return
an enum and map it to a resource key beside the enum, with a `switch` rather than string
concatenation, so renaming the enum is a compile error rather than a run-time missing key.

## Right to left

- `"rightToLeft": "auto"` in `Default.json`, `RightToLeftLayout = true` on the container, every
  child left on `RightToLeft.Inherit`. Set the container, leave the children alone.
- **Pin identifiers to LTR.** Customer codes, IBANs, part numbers - anything a user transcribes.
  Latin letters and digits are neutral characters and an RTL paragraph may reorder them. Names and
  addresses are prose and must mirror.
- `DataGridView` **does** mirror: column order reverses and the row indicator moves. Cell contents
  align by content, so a phone number ends up left-aligned. Set a column's alignment explicitly if
  a number must read the same for everyone.

## Pseudo-localization

`qps-ploc` is a test tool, not a language. Bracket and lengthen every value, then:

- Anything **unbracketed** on screen never went through a resource.
- Anything **clipped** is a layout that only just fits English.

It is the only thing that catches a key missing from a *translation*, because that falls back to
neutral silently. A missing-key policy catches the opposite case - a key nobody ever wrote. **Run
both; neither sees the other's defect.** Demonstrated: a key absent from `Strings.qps-ploc.resx`
rendered in plain English while the policy reported "No missing keys this session", both correct.

## Known gap: overriding Wisej system labels

Wisej ships its own strings - MessageBox buttons are `yes`/`no` in `Wisej.Resources` - translated
for de, es, fr, it, ja, ko, pl, pt, pt-BR, ru, tr, cs, zh-Hans, zh-Hant. **Check before
translating a framework label.**

Overriding one from the application did not work here. `Resources-de.resx` lands in the main
assembly (the hyphen is not a culture); `Resources.de.resx` lands correctly in the `de` satellite
as `GlobalDesk.Resources.de` but Wisej still uses its own. Wisej's are under base name
`Wisej.Resources` and an application resource gets its root namespace prefixed, so the names do not
meet. Left in the sample with the evidence in `Module 3/.../docs/TextKinds.md`.

## Building the screen the walkthrough shows

The videos measure in CSS pixels; Wisej sizes fonts in points. `Desk.Px(15)` in `DeskTheme.cs` is
`new Font("default", 15 * 0.75f)`, and a value copied straight out of a mock then lands where the
mock put it. `Desk.Mono` is the same for the code font the previews use to compare separators.

- **Docking order is reverse of `Controls.Add`.** The control added *last* docks *first*, so it
  claims the outer edge. A `Fill` control is added first and gets what is left. Three `Top`
  panels added in the order bottom, middle, top read down the screen correctly.
- **Do not set `Size` on the `Page`.** A `MainPage` with an explicit size stops filling the
  browser and the window gains a scrollbar. Let it fill.
- **`Anchor` measures its margin from the design-time size**, which the browser is never going to
  match, so an anchored control placed at `x = 1086` ends up off-screen. Put it in a panel docked
  to the edge instead.
- `CssStyle` is the way to a `border-radius`, a one-sided border or `text-transform`; there is no
  property for any of them. `border-inline-end` rather than `border-right` keeps a rail's divider
  on the correct side under RTL.
- An `AutoSize` `Label` docked `Left` **wraps** instead of growing. Give it an explicit width, or
  the longest translation turns a one-line heading into two.
- `Application.RightToLeft` is a **`bool`** in Wisej, not a `RightToLeft` enum value.
- `CultureInfo.GetCultureInfo("qps-ploc").Name` comes back as **`qps-Ploc`**. Compare culture
  names with `StringComparison.OrdinalIgnoreCase` or a picker built from the lower-case string
  reports the session's culture as unknown.
- `Wisej.Resources.ResourceManager.GetString(key, culture)` exists, and
  `GetString(key, CultureInfo.InvariantCulture)` reads the neutral value - which is all an
  "untranslated" report needs.

## Other gotchas

- A `--` inside an XML comment breaks the `.csproj` with `MSB4025`.
- Assigning `ComboBox.SelectedItem` raises `SelectedIndexChanged`; guard re-entry with a flag.
- In C#, an escaped quote cannot appear inside an interpolation hole. Compute the value into a
  local first.
- The designer already defines `Dispose(bool)`; add to it rather than writing a second one.
