# GlobalDesk · Localization in Wisej.NET · Module 5

Local lab build for **Module 5 · Right-to-Left and Layout That Survives Translation**. Arabic in
the picker with `rightToLeft: auto`, `RightToLeftLayout` on the dashboard and the editor with every
child left on `Inherit`, one control deliberately pinned left-to-right, a pseudo-localized resource
set for the long-string pass, and a contacts grid so the module can record what a `DataGridView`
really does under RTL rather than assuming.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 5/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6105
```

Open <http://localhost:6105>, or go straight to <http://localhost:6105/?lang=ar-EG>.

## What to try

| Action | Expected result |
|---|---|
| Pick **ar-EG** | The whole screen mirrors. The status line reports `RightToLeft: True`, set by `rightToLeft: auto` and not by any code. |
| Type a name and a customer code under Arabic | The name is right-aligned; `AT0417` is **left-aligned**. That is the one pinned control. |
| Look at the contacts grid under Arabic | Columns run right to left and the row indicator moves - but the phone numbers align left, because their content decides. |
| Notice the editor under Arabic | Still English. There is no `CustomerEditor.ar.resx`; designer resources fell back. Silent, and the screen looks deliberate. |
| Pick **qps-ploc** | `[Wëlcomë to GlobalDësk ~~~]`. Every caption bracketed and ~40% longer. Anything **not** bracketed never went through a resource. |
| Drag the browser narrow | The action bar wraps onto a second row. Nothing goes off the edge. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Arabic in the picker, `rightToLeft: auto` | `Cultures` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs), [Default.json](GlobalDesk/Default.json) |
| `RightToLeftLayout` on both containers, children on `Inherit` | [DashboardPage.Designer.cs](GlobalDesk/DashboardPage.Designer.cs), [CustomerEditor.Designer.cs](GlobalDesk/CustomerEditor.Designer.cs) |
| One deliberately left-to-right control, with the reason | `txtCode.RightToLeft = RightToLeft.No` in the editor designer |
| Layout rebuilt so long translations cannot clip | `FlowLayoutPanel` + `AutoSize` + `TableLayoutPanel` + docking |
| Pseudo-localized resource set | [Resources/Strings.qps-ploc.resx](GlobalDesk/Resources/Strings.qps-ploc.resx) |
| Five-case test list and the grid's real behaviour | [docs/RtlTests.md](GlobalDesk/docs/RtlTests.md) |

## Notes for anyone extending the RTL work

- **Set the container, leave the children alone.** `RightToLeftLayout` on the page plus
  `rightToLeft: auto` mirrors everything. A screen where each control carries its own setting is a
  screen where one gets missed.
- `auto` means the application never keeps a list of which languages are right-to-left.
- **Pin identifiers to LTR.** Customer codes, IBANs, part numbers, licence keys - anything a user
  transcribes. Latin letters and digits are neutral characters and an RTL paragraph is entitled to
  reorder them. Names and addresses are prose and must mirror.
- The `DataGridView` **does** mirror - column order and the row indicator - but cell contents align
  by content. Set a column's alignment explicitly if a number must read the same for everyone.
- **Designer resources and shared resources are two separate translation jobs.** `Strings.ar.resx`
  being complete does not translate `CustomerEditor`; the fallback is silent and the screen looks
  intentional. Pseudo-localization is what catches it.
- `qps-ploc` is a test tool, not a language. Nobody ships it, and it is the cheapest way to find
  both hard-coded strings and layouts that only just fit English.
