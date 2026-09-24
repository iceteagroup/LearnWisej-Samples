# Module 2 lab note - what the German variant overrides, and what it leaves alone

`CustomerEditor` is `Localizable`. Every localizable property has moved out of
`CustomerEditor.Designer.cs` and into `CustomerEditor.resx`, and the German variant lives in
`CustomerEditor.de.resx`. The generated code says nothing about captions or sizes - it says
`resources.ApplyResources(this.btnSave, "btnSave")`.

## Overridden in German - nine entries

| Key | Neutral | German | Why it had to change |
|---|---|---|---|
| `lblTitle.Text` | Customer | Kunde | wording |
| `lblName.Text` | Name | Name | same word, kept for completeness so the file reads as a full translation |
| `lblCode.Text` | Customer code | Kundennummer | wording |
| `lblCountry.Text` | Country | Land | wording |
| `btnSave.Text` | Save | Speichern | wording |
| `btnSave.Size` | 120, 38 | **150, 38** | "Speichern" does not fit 120 px |
| `btnCancel.Text` | Cancel | Abbrechen | wording |
| `btnCancel.Size` | 120, 38 | **150, 38** | "Abbrechen" does not fit either |
| `btnCancel.Location` | 306, 194 | **336, 194** | it follows Save, which is now 30 px wider |

One `Size` and one `Location` at minimum, as the lab asks - in practice two of each, because the
two buttons sit next to each other and widening the first moves the second.

## Deliberately *not* overridden - nineteen entries inherited

Everything else stays in the neutral file and is inherited: the control's own size, every label
`Location` and `Size`, every text box and the combo box, and `lblMessage`.

That restraint is the point. A German `.resx` that repeats all twenty-eight entries looks
thorough and is a maintenance trap: change the neutral layout afterwards and German silently keeps
the old geometry, because an override always wins. **Override only what must differ.** Every entry
you copy without changing is a future divergence you have signed up for.

The test to apply before adding an override: *if the neutral layout changes next month, do I want
German to follow?* If yes, do not override it.

## Why the buttons needed a size override at all

They did not have to. `btnSave` could have been `AutoSize` with a `MinimumSize`, like
`btnCustomers` on the dashboard, and then no German size entry would exist.

Both are legitimate and the choice is worth making on purpose:

- **`AutoSize`** - no per-language geometry, nothing to maintain, and it cannot clip. The cost is
  that button widths vary between languages, so a row of buttons is ragged.
- **A size override** - the designer shows exactly what ships and a reviewer can see the German
  layout in the designer. The cost is one entry per language per control, forever.

This lab uses `AutoSize` on the dashboard and explicit overrides in the editor so both appear in
one application. For a screen with many languages, `AutoSize` plus a layout panel wins; the
dashboard's preview table is a `TableLayoutPanel` for the same reason.

## The behaviour that catches everyone

`ApplyResources` is called from `InitializeComponent`, which runs when the control is
**constructed**. Designer resources are therefore applied **once**, at construction, and a culture
change afterwards does nothing to them.

The lab shows all three behaviours side by side. Switch the culture to `de-DE` and:

- The **formatted values** change immediately - they are formatted on every refresh.
- The **page captions** do not change - there is no `Strings.de.resx` yet. Module 3 adds it, and
  then they will, because `ApplyTextResources` is re-run.
- The **editor** does not change either, for a completely different reason: it was constructed
  under `en-US` and nothing re-reads its resource.

Press **Recreate the editor** and it changes:

```
Editor rebuilt. The old instance was constructed under en-US; the new one under de-DE.
```

`Kunde`, `Kundennummer`, `Land`, `Speichern`, `Abbrechen`, and the two buttons visibly wider.

Two behaviours that look identical from the outside - "the caption did not change" - with two
different causes and two different fixes. Telling them apart is what this module is for.

## How the recreate is done

The editor is never added to the page directly. It lives in `pnlEditorHost`, and recreating means:

```csharp
this.pnlEditorHost.Controls.Clear();
this.editor?.Dispose();
this.editor = new CustomerEditor();
this.pnlEditorHost.Controls.Add(this.editor);
```

Dispose matters - the old instance is a server-side object with a client counterpart, and dropping
the reference without disposing leaks both. A host panel makes this a three-line method instead of
an exercise in finding every place the control might have been added.

In a real editor this is also where you would carry the user's unsaved input across, because
rebuilding the control throws away everything typed into it. That is the real cost of designer
localization with a runtime language switch, and it is why Module 7's `LocalizationService` keeps
run-time text in the shared resource where a switch is free.
