# Module 7 - the theme QA pass, and what it found

Run the Summary page, switch `cboTheme` between `Bootstrap-4` and `BootstrapDark-4`, and look at
every glyph on the page. Findings below are from doing exactly that.

## Pass

| What | Light | Dark | Verdict |
|---|---|---|---|
| `picTheme` - `icon-search` | dark on white | light on dark | follows the theme |
| `picOfficial` - FontAwesome `cog.svg` | dark | light | follows the theme |
| `picCustom` - `AppIcons.Filter` | dark | light | follows the theme, because the pack strokes with `currentColor` |
| `picEmbedded` - `badge.gif` | green | green | fixed, correctly - it is a raster, nothing can recolour it |
| `picRecoloured` - `warning.svg?color=invalid` | red | red | fixed, correctly - `invalid` is red in both themes |
| Grid text and chrome | fine | fine | the theme handles it |

Five image mechanisms, five correct outcomes, no work needed.

## Fail

**The icon-font glyphs do not follow the theme.** In the toolbar card and in the grid's action
column, every glyph stays `#5a6b7d` under the dark theme - dark grey on a dark background, close to
unreadable, while every image-based icon beside them has turned light.

The cause is in `Images/icon-font.css`:

```css
.idi {
    color: #5a6b7d;   /* a literal, and the theme has no idea it exists */
}
```

This is the trap icon fonts bring with them, and it is not a Wisej.NET problem. Glyphs are **text**
styled by a stylesheet the document loads. Nothing about them passes through the theme, so every
colour decision made in that stylesheet is a decision the theme cannot revisit. Image sources get
recoloured for free; icon fonts never do.

### The three ways out, in order of preference

1. **Do not set a colour at all.** Drop `color` from `.idi` and the glyph inherits the colour of
   the text around it, which the theme *does* control. This is the right answer for glyphs that sit
   inside ordinary content, and it is what the lab would ship.
2. **Set the colour from C# when the theme changes.** Handle `Application.ThemeChanged`, rebuild
   the `AllowHtml` content with an inline `style="color: ..."` taken from the theme. Correct, but it
   puts presentation back into application code, which is what the stylesheet was for.
3. **Ship a stylesheet per theme.** Honest for a product with two fixed themes, unmanageable for a
   product where customers pick their own.

The finding is left in the sample on purpose. Fixing it silently would remove the one thing this
page has to teach about icon fonts: they are the only mechanism in this course that a theme switch
cannot reach.

## Also worth recording

- **Hover states are stylesheet-only too.** `.idi-action:hover` uses a literal blue and
  `.idi-danger:hover` a literal red. Both happen to remain legible under the dark theme, but that
  is luck, not design.
- **The glyphs have no accessible name beyond `title`.** A screen reader reads the `title`
  attribute; without it a font glyph is either silence or a meaningless character name. Every
  action glyph on this page carries one. An image-based icon on a Button at least has the button's
  text to fall back on.
- **Grid actions need a hit area, not just a glyph.** `.idi-action` adds padding and a radius so
  there is something to aim at. A bare glyph is a ~13 pixel target, which fails on touch.
- **`e.Role` survives the theme switch**, as it should - the role attribute is in the markup, not
  in the styling.
