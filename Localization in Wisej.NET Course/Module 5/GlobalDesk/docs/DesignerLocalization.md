# Designer localization - the rule, carried forward from Module 2

`CustomerEditor` is `Localizable`. Every localizable property - `Text`, `Size`, `Location` - lives
in `CustomerEditor.resx`, the German differences live in `CustomerEditor.de.resx`, and the
generated code became `resources.ApplyResources(control, "name")`.

Colours, fonts and border styling are still set in the designer's C#. They have no German variant,
and a copy per language is a copy to keep in step forever.

## The rule that catches everyone

`ApplyResources` is called from `InitializeComponent`, which is called from the constructor.
**Designer resources are applied once, when the control is created.** Change
`Application.CurrentCulture` afterwards and an open editor keeps every caption it was born with.

```csharp
host.Controls.Clear();
editor?.Dispose();
editor = new CustomerEditor();
host.Controls.Add(editor);
```

`Dispose` is not optional: the old instance has a client-side counterpart, and skipping it leaks
one control tree per rebuild. The rebuild's real cost is in the first line - whatever the user had
typed is gone.

## Override only what must differ

The German file in this module holds the handful of entries that actually change: the window
title, the captions whose German wording is different, and the button `Size` and `Location` the
longer captions force. `lblName.Text` is **not** in it, because German also says *Name*.

Every entry copied without changing is a future divergence: reword the neutral value and the
language file keeps the old wording, silently, until somebody compares them. Move a control in the
neutral layout and a copied `Location` pins the language to where it used to be.

**A language file contains differences, never a copy of the baseline.**

An `AutoSize` control with a `MinimumSize` avoids per-language geometry entirely. The lab keeps the
explicit `Size` override because seeing it in the `.resx` is the point; a production screen would
usually rather not have one.

## Two translation jobs, not one

The editor's designed captions come from `CustomerEditor.de.resx`. The sentences it produces while
running - the last-order line, the save confirmation, the validation message - come from
`Texts.Get` and the shared `Strings.resx`.

Separate files, separate work, and each falls back silently to the neutral value on its own. A
complete `Strings.de.resx` does not translate this control, and a complete `CustomerEditor.de.resx`
does not translate the messages. A half-translated screen looks entirely intentional.
