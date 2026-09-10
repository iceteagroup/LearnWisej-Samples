# AdaptiveOps · Theming & Responsive Layouts Course · Module 2

Local lab build for **Module 2 · Theme Builder and Theme JSON Internals**. It follows the walkthrough video: the
Module 1 console stops being branded control by control and gets **one theme file**, `Themes/AdaptiveOps.theme`,
created as a full copy of the Bootstrap-4 base theme and edited in the lesson's order — named colour and font
tokens first (`brandPrimary`, `brandAccent`, `surface`, `surfaceAlt`, `textMain`, `textMuted`, `danger`, `warning`,
`success`, `focusFrame`; fonts `default`, `heading`, `mono`), then the button, panel (caption bar), tab, grid header,
invalid editor and tooltip appearances from those tokens, then an `action-button` appearance that inherits from
`button` and overrides only default, hovered and pressed. `Default.json` names the theme, the status bar reports
`Application.Theme.Name`, the primary command gets its look through `AppearanceKey`, and not one `BackColor`,
`ForeColor` or `Font` is left in the project.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 2/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5502
```

Then open <http://localhost:5502>. (Visual Studio: open `AdaptiveOps.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Open the page | startup | page on `surfaceAlt`, white cards with 6 px corners, `brandPrimary` caption bars on the trace and details cards, 18 px metric values, monospaced trace; status bar right: `theme: AdaptiveOps (Themes/AdaptiveOps.theme · Default.json)`; status `● theme AdaptiveOps` in the `success` colour; trace `• server Application.Theme.Name = "AdaptiveOps" (page load)` and two `theme file … ✓` lines |
| **Apply theme** | success | Save (details) and Apply theme turn `brandPrimary` with white text — `AppearanceKey = "action-button"`, no BackColor; hover `#1F4A92`, hold `#183B75` + 1 px shift, Tab = the shared focus ring; the workspace switches to the **Token inspector** tab: 18 rows, *Defined as* = *Resolved* for every colour (`brandPrimary · #2454A6 · #2454A6 (@brandPrimary)`); trace lists the four brand tokens and the three action-button backgrounds |
| **Walk states** | progress | a `Wisej.Web.Timer` resolves one appearance path every 700 ms — 22 lines: `button/backgroundColor [hovered] = #E6EAF2 (@surfaceHover)`, `action-button/backgroundColor [pressed] = #183B75 … transform translate(1px, 1px)`, `action-button/shadowColor [focused] = … ← INHERITED from button`, `action-button/opacity [disabled] = 0.5 ← INHERITED`, `textbox/color [invalid] = #B42318 (@danger)`, `tooltip/atom`, `panel/captionbar`, `tabview/page/button [checked]`, fonts `heading` / `mono`; the toolbar label counts the steps |
| **Missing theme** | failure 1 | `Application.LoadTheme("Missing-Theme")` inside try/catch — the trace records whether it threw or returned and what `Application.Theme.Name` is afterwards; then `GetColor("brandPrimry") = Color.Empty (IsEmpty = true)` and `GetColor("action-buton", "backgroundColor") = Color.Empty`: a misspelt token or key is silent, the widget just paints nothing; red banner, status `● theme error` in `danger`, an AlertBox top-right; the console keeps running |
| **Invalid ticket** | failure 2 | the repository rejects the empty title on the server; `txtTitle.Invalid = true` puts the editor into the theme's **invalid** state — `danger` border, `InvalidMessage` in the error tooltip — nothing coloured in code; banner + status `● validation error` |
| **Reset** | recovery | `Application.LoadTheme("AdaptiveOps")` if another theme is active, action-button re-applied, editor valid again, banner gone, seed tickets back, token inspector refilled, status `● ready` |
| **Base theme ⇄** | comparison | the whole console flips to **Bootstrap-4** live (blue `#007AFF` caption bars, grey `#D1E0E5` hover, 4 px radius); the controls that name a custom appearance (`action-button`, `metric-card`, the label variants) render as bare widgets because Bootstrap-4 does not define them — the trace calls it the "wrong AppearanceKey" pitfall; click again (button now reads `AdaptiveOps ⇄`) to come back. `LoadTheme` is global: every session of this app switches |
| Hover any toolbar button, tab or the Title editor | tooltip | `tooltip/atom` on `textMain` with `surface` text — the tooltip appearance from tokens |
| Click a grid header | grid header | `table-header-cell`: `surfaceAlt`, `textMuted`, `surfaceHover` on hover; the sorted column's text turns `brandAccent` |
| Settings (navigation rail) | – | opens the Token inspector tab; the rail keeps the base `button` appearance |
| Clear trace | – | empties the trace card |

## Where things live

```
Module 2/
├─ AdaptiveOps.slnx
├─ README.md                        this file
└─ AdaptiveOps/
   ├─ Themes/AdaptiveOps.theme      the custom theme — full Bootstrap-4 copy + the Module 2 edits (Content, copied to bin/)
   ├─ Default.json                  "theme": "AdaptiveOps"   ← what the browser receives
   ├─ Web.config                    Wisej.DefaultTheme = AdaptiveOps (same selection, appSettings form)
   ├─ AdaptiveOps.csproj            net10.0-windows;net10.0 · Themes\*.theme copied to the output
   ├─ MainPage.Designer.cs          the docked shell — AppearanceKey / States only, no BackColor·ForeColor·Font
   ├─ MainPage.cs                   lab paths: Apply theme · Walk states · Missing theme · Base theme ⇄ · Invalid ticket · Reset
   ├─ Models/Ticket.cs, TicketRepository.cs   the same ticket data as every module
   ├─ Program.cs / Startup.cs / Default.html  session entry point, Kestrel host, page
   └─ docs/
      ├─ ThemeNotes.md              file structure, every token and appearance with the reason, inheritance and state order
      ├─ ThemeBuilderSteps.md       the Theme Builder session as the video shows it (tree · grid · preview · live update · Last Clicked)
      └─ ThemePreviewScreenshots.md the preview/browser screenshot list (taken by the learner) and what each must show
```

## Deliverables

1. **`AdaptiveOps.Theme.json` created from a base theme and loaded by the application** —
   [`AdaptiveOps/Themes/AdaptiveOps.theme`](AdaptiveOps/Themes/AdaptiveOps.theme) (the framework loads `/Themes/<name>.theme`),
   selected in [`Default.json`](AdaptiveOps/Default.json) and [`Web.config`](AdaptiveOps/Web.config); how it was created: [`docs/ThemeBuilderSteps.md`](AdaptiveOps/docs/ThemeBuilderSteps.md)
2. **Named colour tokens and default / heading / mono font tokens defined once and reused** — `colors` and `fonts` in the theme file; table with every use in [`docs/ThemeNotes.md`](AdaptiveOps/docs/ThemeNotes.md) §3; live values in the app's Token inspector
3. **Button, panel, tab, grid header, invalid editor and tooltip appearances restyled from the tokens** — [`docs/ThemeNotes.md`](AdaptiveOps/docs/ThemeNotes.md) §4 (JSON of each)
4. **`action-button` inheriting from `button` with overridden default, hovered and pressed** — theme file + [`docs/ThemeNotes.md`](AdaptiveOps/docs/ThemeNotes.md) §4–§6; applied in `MainPage.cs` `ApplyActionButton()`
5. **Theme Builder preview screenshots and the note on state order and inheritance decisions** — [`docs/ThemePreviewScreenshots.md`](AdaptiveOps/docs/ThemePreviewScreenshots.md) (screenshot list) and [`docs/ThemeNotes.md`](AdaptiveOps/docs/ThemeNotes.md) §5–§6 (the note); theme-vs-CSS-vs-layout decisions and the AI-assistant question in §7 and §9

## Lab step → code map

| Lab step (`labs/m2.json`) | Where |
|---|---|
| 1 · open the Module 1 solution, run it on the base theme, confirm shell + width label | `Base theme ⇄` shows the same console on Bootstrap-4; `MainPage_Load` → `VerifyShell()` / `ReportWidth()` still run |
| 2–3 · new theme from the base theme, named `AdaptiveOps`, saved in `Themes`, committed untouched | `Themes/AdaptiveOps.theme` (full Bootstrap-4 copy); `ThemeBuilderSteps.md` Step 1; `ThemeNotes.md` §1 explains why a copy and not a top-level inherit |
| 4 · ten named colours + `default`/`heading`/`mono` fonts, `focusFrame` following `brandPrimary` | `colors` / `fonts` in the theme file; `ThemeNotes.md` §3 (and the note on why `focusFrame` holds the same literal value) |
| 5 · Last Clicked → restyle button, panel + caption bar, tab, grid header, invalid editor, tooltip from tokens | `ThemeBuilderSteps.md` Step 3 (paths); the JSON in `ThemeNotes.md` §4; visible: toolbar/rail buttons, the two headered cards, the workspace tabs, both grids, `Invalid ticket`, any tooltip |
| 6 · `action-button` with `inherit: button`, default first, only default/hovered/pressed; focused and disabled from button | theme file `appearances.action-button`; proven by `Walk states` (INHERITED lines) and `Apply theme` (Tab / disabled) |
| 7 · `"theme": "AdaptiveOps"` in `Default.json`; `btnApplyTheme_Click` reads `Application.Theme.Name` into the status label, sets `AppearanceKey = "action-button"` on `btnSave`, try/catch for a missing or malformed theme | `Default.json`, `Web.config`; `MainPage.cs` `btnApplyTheme_Click`, `ReportTheme`, `ApplyActionButton`, and `MainPage_Load` (banner when the running theme is not AdaptiveOps) |
| 8 · remove every `BackColor`/`ForeColor`/`Font` the theme now owns; write the state-order and inheritance note | `MainPage.Designer.cs` (search finds none); `SetStatus` uses `lblStatus.States`; `ThemeNotes.md` §5–§6 |
| 9 · show every path: primary command in five states, invalid editor with `danger`, tooltip, grid header, and the status message for a theme name that matches no file | `Apply theme` + hover/hold/Tab/`Base theme ⇄`; `Invalid ticket`; any tooltip; grid headers; `Missing theme` |
| 10 · run, status label reports AdaptiveOps, no console errors, compare with the preview screenshots, no literal hex where a token exists | status bar; `ThemePreviewScreenshots.md`; a lint over the edited appearances finds no hex literal (only Bootstrap-4's untouched `table/resize-line` rgba) |

## Self-check answers (lab guide)

- **Marketing changes `brandPrimary` to a new hex value tomorrow. Which files and which lines change in your solution, and what would the answer have been if you had set `BackColor` on each button instead?**
  One file, `Themes/AdaptiveOps.theme`, and in it the `colors` section: the `brandPrimary` line, plus the tokens that
  intentionally carry the same value because this framework build does not resolve a token whose value is another
  token's name — `focusFrame`, `primary`, `highlight`, `activeCaption`, `switchOn`, `menuSelected` — and, if the shade
  should follow, `brandPrimaryHover`, `brandPrimaryPressed` and `focusShadow`. Nothing in C# changes, because every
  appearance references the names: `action-button`'s three states, the panel caption bar, the metric strips, the
  selected tab, the checked button and the focus borders all move together, hovered, pressed and focused included.
  With `BackColor` on each button the answer would have been "every Designer file that has a button" — Module 1
  alone had the value in the four strips and the banner, a real app has it in dozens of places — and the hovered,
  pressed and focused looks would never have existed in the first place, because an application property overrides
  the theme in every state.
- **You moved the `pressed` state above `hovered` in the `action-button` appearance. What does a user see while holding the mouse button down, and why does the order rather than the colour decide it?**
  While the mouse is held on the button the widget is in *both* states, `hovered` and `pressed` (and usually `focused`),
  so both blocks match. The theme engine applies the matching states in list order and the later block wins for the
  values it sets. With `pressed` above `hovered`, `hovered` is applied last, so the user sees `brandPrimaryHover`
  (`#1F4A92`) while pressing and the `translate(1px, 1px)` shift from `pressed` but not its colour. The colours in the
  two blocks are unchanged; only which one is written last changed, which is why the Theme Builder treats a dragged
  state as a behaviour change and why the file keeps `default › hovered › pressed`.
- **A TextBox in the details editor stays grey when it is invalid even though you styled the invalid state. Which three things do you check, in which order, before editing the theme again?**
  First, an **application property on the control** that overrides the theme — `BackColor`, `ForeColor`, `Font`,
  `CssStyle` or a `CssClass` rule set on that TextBox or inherited from a container. Second, the **AppearanceKey**:
  is the control really rendering the `textbox` appearance you edited, or a variant (its own key, a `celleditor`
  context) that has no `invalid` block? Third, the **state**: is `Invalid = true` actually set on the server side
  (the client adds the `invalid` state only then), and is `invalid` last in the state list so `focused` does not
  overwrite the border while the user is in the field? Only after those, the fourth check the lesson adds: is the
  running application **loading the theme you edited** — `Default.json` `"theme"`, `Web.config`, and the name in the
  status bar — because the theme shown in the Visual Studio designer is not what the browser receives.
