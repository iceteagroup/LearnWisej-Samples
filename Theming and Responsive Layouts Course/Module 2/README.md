# AdaptiveOps · Theming & Responsive Layouts Course · Module 2

Lab build for **Module 2 · Theme Builder and Theme JSON Internals**: the Module 1 console gets one theme file,
`Themes/AdaptiveOps.theme`, created as a full copy of the Bootstrap-4 base theme and edited in the lesson's order:
named colour and font tokens first (`brandPrimary`, `brandAccent`, `surface`, `surfaceAlt`, `textMain`, `textMuted`,
`danger`, `warning`, `success`, `focusFrame`; fonts `default`, `heading`, `mono`), then the button, panel (caption
bar), tab, grid header, invalid editor and tooltip appearances from those tokens, then an `action-button`
appearance that inherits from `button` and overrides only default, hovered and pressed. `Default.json` names the
theme, the status label reports `Application.Theme.Name`, the Save command gets its look through `AppearanceKey`, and
no `BackColor`, `ForeColor` or `Font` is left in the project.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 2/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5502
```

Then open <http://localhost:5502> (or open `AdaptiveOps.slnx` in Visual Studio and press F5).

## What to try

- **Startup**: the console renders on AdaptiveOps (page on `surfaceAlt`, white cards with 6 px corners, a
  `brandPrimary` caption bar on the details card); the status bar reads `Theme: AdaptiveOps` and `Width: N px`.
- **Save** (details panel) is an `action-button`: hover it (`#1F4A92`), hold the mouse (`#183B75` and a 1 px shift),
  Tab to it (the shared focus ring from `button`). Its tooltip uses the `tooltip/atom` appearance.
- **Clear the Title** (or Owner) and press Save: the server rejects the ticket and the editor enters the theme's
  `invalid` state (`danger` border, the message in the error tooltip); the status label says why.
- **Grid header**: `table-header-cell` on `surfaceAlt` with `textMuted` text; the sorted column's text is `brandAccent`.
- **Apply theme** re-reads `Application.Theme.Name` into the status label and re-applies `action-button` to Save.
- **Theme not found**: change `"theme"` in `Default.json` to a name with no file in `Themes/` and restart: the
  status label reads `Theme "AdaptiveOps" was not found in the Themes folder; running "…"`.

## Where things live

```
AdaptiveOps/
├─ Themes/AdaptiveOps.theme      the custom theme: full Bootstrap-4 copy + the Module 2 edits (Content, copied to bin/)
├─ Default.json                  "theme": "AdaptiveOps"
├─ Web.config                    Wisej.DefaultTheme = AdaptiveOps
├─ MainPage.Designer.cs          the docked shell: AppearanceKey / States only, no BackColor, ForeColor or Font
├─ MainPage.cs                   ApplyTheme() (Load + btnApplyTheme_Click), tickets, Save with the invalid state
├─ Models/                       the same ticket data as every module
└─ docs/
   ├─ ThemeNotes.md              file structure, tokens, appearances, inheritance and state-order decisions
   ├─ ThemeBuilderSteps.md       the Theme Builder session (tree, property grid, preview, live update, Last Clicked)
   └─ ThemePreviewScreenshots.md the preview and browser screenshots the learner takes, and what each must show
```

## Lab step → code map

| Lab step | Where |
|---|---|
| New theme from the base theme, named `AdaptiveOps`, saved in `Themes` | `Themes/AdaptiveOps.theme`; `docs/ThemeBuilderSteps.md` Step 1 |
| Ten named colours + `default` / `heading` / `mono` fonts | `colors` / `fonts` in the theme file; `docs/ThemeNotes.md` §3 |
| Restyle button, panel + caption bar, tab, grid header, invalid editor, tooltip from tokens | theme file; JSON in `docs/ThemeNotes.md` §4 |
| `action-button` with `inherit: button`, only default / hovered / pressed | theme file `appearances.action-button`; `docs/ThemeNotes.md` §5–§6 |
| `"theme": "AdaptiveOps"` in `Default.json`; `btnApplyTheme_Click` (or Load) reads `Application.Theme.Name` into the status label, sets `btnSave.AppearanceKey = "action-button"`, try / catch | `Default.json`, `Web.config`; `MainPage.cs` `ApplyTheme()` called from `MainPage_Load` and `btnApplyTheme_Click` |
| Remove every `BackColor` / `ForeColor` / `Font` the theme owns; write the state-order note | `MainPage.Designer.cs` has none; `docs/ThemeNotes.md` §5–§6 |
| Show every path: primary command states, invalid editor, tooltip, grid header, theme-not-found message | Save (hover / hold / Tab), Save with an empty Title, the Save tooltip, grid headers, a wrong name in `Default.json` |

## Self-check answers (lab guide)

- **Marketing changes `brandPrimary` tomorrow. Which files and lines change, and what if you had set `BackColor` on
  each button?** One file, `Themes/AdaptiveOps.theme`, in its `colors` section: `brandPrimary` plus the tokens that
  intentionally carry the same value because this framework build does not resolve a token whose value is another
  token's name (`focusFrame`, `primary`, `highlight`, `activeCaption`, `switchOn`, `menuSelected`) and, if the shades
  should follow, `brandPrimaryHover`, `brandPrimaryPressed`, `focusShadow`. No C# changes. With `BackColor` on each
  button the answer would be "every Designer file with a button", and hovered, pressed and focused would never have
  existed, because an application property overrides the theme in every state.
- **You moved `pressed` above `hovered` in `action-button`. What does the user see while holding the mouse, and why
  does the order decide it?** While held, the widget is both `hovered` and `pressed`; matching states apply in list
  order and the later one wins. With `pressed` above `hovered`, `hovered` is applied last, so the user sees
  `brandPrimaryHover` while pressing (plus the translate from `pressed`). Same colours, different behaviour.
- **A TextBox stays grey when invalid although you styled the invalid state. What do you check, in order?** First an
  application property on the control (`BackColor`, `ForeColor`, `Font`, `CssStyle`, `CssClass`). Second the
  `AppearanceKey`: is it really the `textbox` appearance you edited? Third the state: is `Invalid = true` set on the
  server, and is `invalid` last so `focused` does not overwrite it? Then whether the running app loads the theme you
  edited (`Default.json`, `Web.config`, the name in the status label).
