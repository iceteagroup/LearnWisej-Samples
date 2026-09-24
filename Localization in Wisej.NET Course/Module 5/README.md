# GlobalDesk · Localization in Wisej.NET · Module 5

Local lab build for **Module 5 · Right-to-Left Languages and Localization-Safe Layouts**.
The GlobalDesk dashboard exactly as the walkthrough shows it: the blue application bar, the
navigation column (**Dashboard / Customers / Contacts / Settings**), the **Welcome back, Dana**
greeting, the **Customer code** field, the contacts grid with **Contact / Company / City**, the
**Save changes** / **Cancel** pair, and the status strip reporting the culture and the reading
direction.

Switch the picker to العربية and the whole arrangement mirrors — except the customer code, which
stays left to right because it is an identifier.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 5/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6105
```

Open <http://localhost:6105>, or go straight to a culture with
<http://localhost:6105/?lang=ar-SA> / `?lang=qps-ploc` in a **new tab**.

## What to try

| Action | Expected result |
|---|---|
| Pick **العربية (السعودية)** | Navigation column moves to the right, the grid's columns reverse, the buttons swap order, every caption is Arabic. The status strip reads `الاتجاه: من اليمين إلى اليسار`. |
| Look at the customer code | `GD-40117-AR`, still left to right, still left-aligned. `txtCustomerCode.RightToLeft = No`. |
| Look at the contacts grid under Arabic | Arabic contact names beside Latin company names. Record what it does; see the lab note. |
| Pick **Pseudo (qps-ploc)** | Every caption bracketed and about 40% longer. Anything **not** bracketed never went through a resource. |
| Pick **Deutsch** | `Kontaktverzeichnis` fits the navigation column. A fixed 146 px column is where that word gets cut. |
| Drag the window narrow | The button row wraps instead of pushing a button off the edge. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Arabic in the picker, `rightToLeft: "auto"` | `cultures` in [DashboardPage.cs](GlobalDesk/DashboardPage.cs), [Default.json](GlobalDesk/Default.json) |
| `RightToLeftLayout` on the page and the customer screen | constructors of `DashboardPage` and [CustomerEditor.cs](GlobalDesk/CustomerEditor.cs) |
| Every child left on `RightToLeft.Inherit` | nothing sets it, which is the point |
| `RightToLeft.No` on the customer code, with the reason | [CustomerEditor.Designer.cs](GlobalDesk/CustomerEditor.Designer.cs) |
| Fixed positions replaced by docking, anchoring and a container layout | `pnlNav`, `pnlCodeRow`, `pnlButtons` — no `Location` anywhere on this screen |
| Pseudo-localized resource file | [Strings.qps-ploc.resx](GlobalDesk/Resources/Strings.qps-ploc.resx) and [CustomerEditor.qps-ploc.resx](GlobalDesk/CustomerEditor.qps-ploc.resx), both generated from the neutral values |
| The five-case layout test list, and what the grid really did | [docs/RtlTests.md](GlobalDesk/docs/RtlTests.md) |

## Notes

- `RightToLeftLayout` on the container mirrors the children. **Set the container, leave the
  children alone.** A screen where every control carries its own `RightToLeft` is a screen where
  one of them will be missed.
- Nothing in the C# tests for Arabic. `rightToLeft: "auto"` derives the direction from the
  session's culture, and `Application.RightToLeft` reports it.
- The window glyphs are the one container pinned out of the mirroring: they are chrome, not
  content, and they stay on the right in every language.
- The German file for the customer screen has **no `Size` or `Location` entries any more**. The
  row and the button bar flow and the buttons are `AutoSize` with a `MinimumSize`, so a longer
  caption costs nothing. Compare it with Module 4's.
- Four resource files per language now, not one: the shared `Strings.<lang>.resx` and the
  designer's `CustomerEditor.<lang>.resx`. Counting them is the cheapest way to spot a language
  that was half done.
