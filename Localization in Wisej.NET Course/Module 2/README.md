# GlobalDesk · Localization in Wisej.NET · Module 2

Local lab build for **Module 2 · Localizing Wisej.NET UI in the Visual Studio Designer**.
The GlobalDesk customer editor exactly as the walkthrough shows it: the blue application bar, the
host strip carrying `Application.CurrentCulture` and the **Recreate editor** button, the
**Customer details** group with the customer code, name and city rows, the notes box and the
**Cancel** / **Save changes** pair. `CustomerEditor` is a designer-localized `UserControl`: its
captions, sizes and positions live in `CustomerEditor.resx`, with nine German overrides in
`CustomerEditor.de.resx`.

Module 2 is the editor, so the session opens on it. The Module 1 dashboard returns in Module 4,
with this editor embedded in it beside the language picker.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 2/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6102
```

Open <http://localhost:6102>. In Visual Studio, open `GlobalDesk.slnx` and press F5.

## What to try

| Action | Expected result |
|---|---|
| Read the screen | `Customer details`, `Customer code`, `Name`, `City`, `Notes`, `Cancel`, `Save changes`. Not one of them is in the C#. |
| Click the culture chip (`en`) | It turns amber and reads `de-DE`. **The editor does not change.** Its resources were applied when it was constructed. |
| Read the status line | `Session culture is now de-DE. The editor on screen was built for en and has not changed.` |
| Click **Recreate editor** | German captions, a wider `Änderungen speichern`, a wider `Abbrechen`, and the window title changes with them. |
| Look at `Name` under German | Still `Name`. German inherits it, because `CustomerEditor.de.resx` overrides only what must differ. |
| Type in Notes, then recreate | The text is gone. That is the real cost of the rebuild, and the reason Module 4 does it in one step with the switch. |
| Search the C# for `"Save changes"` | Nothing. The designer file says `resources.ApplyResources(this.btnSave, "btnSave")`. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Neutral English layout of the editor | [CustomerEditor.resx](GlobalDesk/CustomerEditor.resx) — 33 entries |
| `Localizable` set, `ApplyResources` in `InitializeComponent` | [CustomerEditor.Designer.cs](GlobalDesk/CustomerEditor.Designer.cs) |
| German captions plus one `Size` and one `Location` override | [CustomerEditor.de.resx](GlobalDesk/CustomerEditor.de.resx) — nine entries |
| `btnRecreateEditor` that removes, disposes and rebuilds, in try / catch | `CreateEditor` and `btnRecreateEditor_Click` in [CustomerEditorPage.cs](GlobalDesk/CustomerEditorPage.cs) |
| Lab note listing what German overrides and what it inherits | [docs/DesignerLocalization.md](GlobalDesk/docs/DesignerLocalization.md) |

## Notes for anyone extending the editor

- `ApplyResources` runs inside `InitializeComponent`, so **designer resources are applied at
  construction**. A culture change does nothing to an existing instance.
- The host panel is what makes the rebuild three lines: clear it, dispose the old instance,
  construct a new one, add it back. `Dispose` matters — the old instance has a client counterpart.
- Add controls only with the **default** language selected in the designer. A control added while
  German is selected exists in the German resource and nowhere else.
- Colours, fonts and border styling are set in the designer's C#, not in the resource file: they
  have no German variant, and a copy per language is a copy to keep in step forever.
- The window title comes from the control's own `$this.Text` in this module, because the screen
  *is* the control and the title has to change in the same instant its captions do. Module 3 moves
  the shared sentences into `Strings.de.resx`, where a culture change reaches them without a
  rebuild.
- `culture` is pinned to `en` in `Default.json` so the demo starts from a known state. Module 4
  changes it to `auto`.
