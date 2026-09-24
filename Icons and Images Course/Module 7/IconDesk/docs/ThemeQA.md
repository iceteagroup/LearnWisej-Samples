# Module 7 - the theme QA pass, and what it found

Run on the capstone pages under `Bootstrap-4` and `BootstrapDark-4`.

## The five mechanisms, and which of them moved

| Tile | Source | Moved? |
|---|---|---|
| Save | `icon-save` - a theme image | **yes** |
| Search | `resource.wx/Wisej.Ext.FontAwesome/search.svg` - an official pack | **yes** |
| Customer | `AppIcons.Customer` - our own pack | **yes** |
| Logo | `resource.wx/IconDesk/logo.svg` - embedded in the application | no |
| Alert | `…/status-warning.svg?color=invalid` - a recoloured SVG | **yes** |

The intuitive expectation - the theme owns its own images and everything else is a fixed file - is
wrong, and this table is the evidence. Four of the five moved, and the one that did not is the
asset *most* under our control.

What decides is the artwork, not the mechanism. Wisej.NET inlines an SVG image source as a
`data:image/svg+xml;base64` background and injects `fill` and `color` on the root element:

- `search.svg` and `customer.svg` set no fill of their own, so the injected fill paints them and
  they change with the theme.
- `logo.svg` carries **two** meaningful fills, so Wisej.NET reads it as artwork and passes it
  through untouched. A brand mark should be written that way on purpose.
- The recoloured warning names a theme colour, so it resolves again under the new theme.

Decide, per asset, whether you want it repainted - and then write the file so that is what
happens.

## The finding the pass exists to catch

Icon-font glyphs are **not** images. They are text, styled by `Images/icon-font.css`, rendered
inside `AllowHtml` content. Nothing about them passes through the theme.

The first version of that stylesheet set a colour:

```css
.idi { color: #5a6b7d; }
```

Under `BootstrapDark-4` that produced dark-grey glyphs on a dark grid, sitting beside image icons
that had correctly turned light. No exception, no warning, no failed request - just an action
column a user cannot read.

The fix is to set no colour at all and let the glyph inherit from the text around it. That is the
rule: **a colour set in CSS is a colour no theme can revisit.** If a glyph genuinely needs its own
colour - the delete action's hover state, here - scope it to that state rather than to the glyph.

## The rest of the checklist

| Check | Result |
|---|---|
| Every image resolves in both themes | yes - no failed requests in the network panel |
| Every icon has a visible name or a tooltip | yes - the tiles carry both, the grid glyphs carry `title` and a printed role |
| Contrast in the dark theme | glyphs and pills read; the only failure was the CSS colour above |
| `?color=` names resolve | `invalid`, `info`, `success` and `highlight` do; `error` and `activeText` do not |
| Grid actions distinguishable | `role='edit'` and `role='delete'` are reported separately by one handler |

A QA pass is not an assertion. Two themes, one screen, and somebody looking at it - which is why
the findings live in this note rather than in a test.
