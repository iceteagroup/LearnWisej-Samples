# GlobalDesk · Localization in Wisej.NET · Module 2

Local lab build for **Module 2 · Designer Localization**. A `CustomerEditor` user control with
`Localizable` set to true over a complete neutral English layout, a German variant that overrides
only what must differ, and a Recreate button that shows why designer resources do not follow a
runtime culture change.
The Module 1 dashboard is unchanged and now hosts the editor.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 2/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6102
```

Open <http://localhost:6102>.

## What to try

| Action | Expected result |
|---|---|
| Switch the culture to **de-DE** | The formatted values turn German immediately. The editor's captions do not. Nor do the page's own captions - `Strings.de.resx` arrives in Module 3. |
| Read the status line | It names all three behaviours and does not pretend the editor changed. |
| Press **Recreate the editor** | `Kunde`, `Kundennummer`, `Land`, `Speichern`, `Abbrechen` - and the two buttons visibly wider, with Cancel moved right. |
| Switch back to **en-US** and press Recreate again | Back to the neutral layout, including the narrower buttons. |
| Type a name, then Recreate | The text is gone. Rebuilding the control throws away everything in it - the real cost of this approach. |
| Open `CustomerEditor.de.resx` | Nine entries. Everything else is inherited from the neutral file on purpose. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `CustomerEditor` with `Localizable` over a neutral English layout | [CustomerEditor.Designer.cs](GlobalDesk/CustomerEditor.Designer.cs) + [CustomerEditor.resx](GlobalDesk/CustomerEditor.resx) |
| German variant overriding captions plus a Size and a Location | [CustomerEditor.de.resx](GlobalDesk/CustomerEditor.de.resx) |
| `AutoSize`, docking or anchoring so captions cannot clip | `btnCustomers` and `layoutPreview` on the dashboard; the editor uses explicit overrides instead, and the note says why |
| Recreate button that disposes and rebuilds the container | `CreateEditor` / `btnRecreate_Click` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs) |
| Lab note listing overridden and inherited properties | [docs/DesignerLocalization.md](GlobalDesk/docs/DesignerLocalization.md) |

## Notes for anyone extending the editor

- **Designer resources are applied once, at construction.** `ApplyResources` runs inside
  `InitializeComponent`. A culture change afterwards does nothing until the control is rebuilt.
- **Override only what must differ.** A German file that repeats every neutral entry silently
  keeps the old geometry when the neutral layout changes.
- Widening one button moves the one beside it. That is why `btnCancel.Location` is in the German
  file as well as its `Size`.
- `AutoSize` with a `MinimumSize` avoids per-language geometry entirely, at the cost of ragged
  button widths. Both approaches are in this sample on purpose.
- Rebuilding the control loses unsaved input. Carry it across in a real editor, and keep run-time
  messages in the shared resource - they follow a culture change for free.
- Validation and confirmation messages come from `Texts`, not from the control's designer
  resource. They are produced at run time and belong to the application, not to one screen's
  design.
