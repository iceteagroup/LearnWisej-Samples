# Module 2 lab note - what German overrides, and what it deliberately inherits

`CustomerEditor` is `Localizable`. Every localizable property moved out of
`CustomerEditor.Designer.cs` and into `CustomerEditor.resx`, and the generated code became
`resources.ApplyResources(control, "name")`.

## The neutral file: 33 entries

The whole screen - text, position and size for eleven controls plus `$this.Size` and `$this.Text`.
It is the file every other language falls back to, so it has to be complete.

## The German file: nine entries

| Entry | Neutral | German | Why |
|---|---|---|---|
| `$this.Text` | GlobalDesk — Customer editor | GlobalDesk — Kundeneditor | The window title of this screen. |
| `lblDetails.Text` | Customer details | Kundendaten | |
| `lblCode.Text` | Customer code | Kundennummer | |
| `lblCity.Text` | City | Ort | |
| `lblNotes.Text` | Notes | Bemerkungen | |
| `btnCancel.Text` | Cancel | Abbrechen | |
| `btnCancel.Size` | 88 × 38 | 118 × 38 | *Abbrechen* is four characters longer than *Cancel*. |
| `btnSave.Text` | Save changes | Änderungen speichern | |
| `btnSave.Size` | 128 × 38 | 186 × 38 | The neutral width cuts the German caption in half. |

Plus the two `Location` entries the widths drag along with them - see below.

## What German does *not* override

`lblName.Text` is the obvious one: German also says **Name**. Copying it into the German file
would look tidier and would be a slow-motion defect. Reword the neutral value later and the
German file keeps the old wording, silently, for as long as nobody compares them.

The same goes for every `Location` and `Size` that did not have to change: nineteen entries are
inherited. Each one copied would be one more place to edit when the neutral layout moves.

**The rule: a language file contains differences, never a copy of the baseline.**

## Why a Size override brings a Location with it

The two buttons sit at the bottom right. Widening `btnSave` from 128 to 186 pushes its left edge
58 px to the left, and `btnCancel` has to move with it, so German also carries
`btnSave.Location` and `btnCancel.Location`.

The walkthrough's override list names `lblCode.Location` at this point. The mechanism is the same
one - a language that needs different geometry stores it against that language alone - but in this
layout the control that actually has to move is the button pair, so that is where the overrides
are. Anchoring the buttons to the bottom right, or an `AutoSize` button with a `MinimumSize`,
avoids per-language geometry entirely; the lab keeps the explicit override because seeing it in
the `.resx` is the point.

## The rule that catches everyone

`ApplyResources` is called from `InitializeComponent`. `InitializeComponent` is called from the
constructor. **Designer resources are therefore applied once, when the control is created.**

Click the culture chip with the editor open and watch: the chip turns amber, the status line says
the session is now `de-DE`, and the editor keeps every English caption it was born with. Nothing
is broken. Nothing will fix itself either.

```csharp
this.pnlEditorHost.Controls.Clear();
this.editor?.Dispose();
this.editor = new CustomerEditor();
this.pnlEditorHost.Controls.Add(this.editor);
```

`Dispose` is not optional: the old instance has a client-side counterpart, and skipping it leaks
one control tree per rebuild.

The rebuild's real cost is in the first line. Whatever the user had typed into Notes is gone.
A production screen saves and restores the values around the rebuild, or does not offer the
language switch while an editor is open. Module 4 does the rebuild inside the `CultureChanged`
handler so the half-translated state never appears at all.

## Two translation jobs, not one

The editor's captions come from `CustomerEditor.de.resx`. The messages it produces at run time -
the save confirmation, the validation message - come from `Texts.Get` and the shared
`Strings.resx`. They are separate files and separate work.

That matters in the direction people forget: a complete `Strings.de.resx` does **not** translate
this control, and a complete `CustomerEditor.de.resx` does not translate the messages. Each falls
back silently to the neutral value, and a half-translated screen looks entirely intentional.

You can see it in this module: with the session on `de-DE` the rebuilt editor is German while the
**Recreate editor** button above it is still English, because the shared German file does not
exist until Module 3.
