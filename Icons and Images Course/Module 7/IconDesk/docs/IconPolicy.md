# IconDesk production icon policy

The rules this project follows, and the reasoning behind each one. Everything here is backed by
something the labs in this course actually demonstrated.

## 1. Which mechanism, for what

| Situation | Mechanism | Why |
|---|---|---|
| An interface icon on a control | `ImageSource` naming an icon-pack asset | Vector, cacheable, recoloured by the theme, no server memory |
| An icon the active theme already defines | `ImageSource` with the theme name | Comes from the theme cache the browser already holds |
| A picture the code produced or received as a .NET object | `Image` | It is already an object; anything else means re-encoding it |
| The same concept on many controls | `ImageList` + `ImageKey` | One edit changes every user of the key |
| A product asset that must be exactly what we shipped | Embedded resource, **qualified** URL | Nothing on a deployment's disk can replace it |
| An asset a deployment is meant to replace | Embedded resource, **unqualified** URL | A file beside the executable wins; no rebuild |
| A decorative or repeated glyph in HTML content | Icon font | Free to size and colour in CSS - but read section 5 |

The primary icon family is **FontAwesome**, via `Wisej-4-FontAwesome`. Material Design is
installed and may be used where FontAwesome has no icon for a concept; every such exception is
recorded in the pull request that introduces it. Project-specific icons go in `IconDesk.Icons`.

**Never copy an icon file into a screen's folder.** If it belongs to the product it belongs in the
pack.

## 2. Accessibility

- Every interactive icon carries a text label or a `title`. A glyph with neither is silent to a
  screen reader and ambiguous to everyone else.
- Colour never carries meaning on its own. The `warning` icon is a triangle as well as red; a
  status column shows a word beside the chip.
- Icon-only buttons need a tooltip and an accessible name. Prefer icon **and** text for anything
  destructive.
- An interactive glyph needs a hit area, not just a glyph. `.idi-action` adds padding for that
  reason; a bare 13-pixel character fails on touch.
- Anything that exists only as a picture needs a textual equivalent somewhere on the page.

## 3. Licensing

- Only packs whose licence permits redistribution in a compiled application. The official Wisej-4
  packs are referenced as NuGet packages and their licences travel with them.
- Icons drawn for this project live in `IconDesk.Icons` and are ours. Icons taken from anywhere
  else are recorded in that project's README with their source and licence **before** they are
  committed.
- No icon enters the product from a search-engine image result. If the provenance is unknown the
  icon does not ship.

## 4. Caching and cost

- Icon-pack and embedded-resource URLs are stable and cacheable. Measured in Module 4: ten icons
  produce exactly ten requests on first view and none afterwards. Installing a 519-icon pack to use
  twelve of them costs twelve requests, once per browser.
- `Image` assets cost server memory and a PNG encode per distinct image, and they are re-sent per
  session. Use them only when the code genuinely owns an image object.
- When the bytes behind a resource URL change, the URL must change too - a `?v=` parameter or a
  rename. Otherwise returning users keep the old picture. This bites at deployment time, not in
  development.
- Do not defeat caching to make development convenient. Cache-busting belongs in the lab, not in
  the product.

## 5. External dependencies

- **No icon CDN.** Every asset is served by our own application, from an assembly we deploy. A CDN
  adds a third party to the availability of our interface, leaks user traffic to them, and breaks
  in air-gapped installations - and some of our customers run air-gapped.
- The icon-font stylesheet ships with the application, is referenced from `Default.html`, and uses
  no remote font. If a bespoke webfont is introduced it is embedded and served by us.
- **Icon fonts do not follow the theme.** Glyphs are text styled by a stylesheet; nothing about
  them passes through Wisej.NET's theming, so a colour set in CSS is a colour no theme can revisit.
  Module 7's QA pass found exactly that - see `ThemeQA.md`. Therefore: do not set `color` on icon
  glyph classes; let them inherit from the surrounding text, which the theme does control.
- Prefer an image source over a font glyph for anything semantic. Icon fonts are for decorative and
  inline uses where CSS control is worth losing theme integration.

## 6. Review checklist

Before an icon change merges:

- [ ] Uses a mechanism from the table in section 1, and the obvious one for the case.
- [ ] From the primary family, or the exception is stated in the pull request.
- [ ] Provenance and licence known and recorded if the artwork is new.
- [ ] Monochrome SVG strokes or fills with `currentColor` so the theme can recolour it, with
      `fill="none"` on every shape - `fill="none"` on a wrapping `<g>` is not enough, the
      injected root fill wins and the outline comes back solid.
- [ ] Artwork that must **not** be repainted - a brand mark - carries two or more meaningful
      fills, which is what makes Wisej.NET pass it through untouched.
- [ ] Any `?color=` value names a theme colour the theme really defines. `invalid`, `info`,
      `success`, `highlight` and `hotTrack` resolve; `error` and `activeText` do not, and an
      unknown name is a silent no-op.
- [ ] No `width`/`height` on the root element - `viewBox` only.
- [ ] Accessible name present for anything interactive.
- [ ] Checked in a light **and** a dark theme.
- [ ] No new external host.
