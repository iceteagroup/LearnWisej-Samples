# IconDesk.Icons - the pack's rules

Written from building the pack and running IconDesk against it, not from the documentation.

## What the pack is

One class library, `IconDesk.Icons`, with no Wisej dependency at all. Every icon is an SVG under
`Resources/` marked `Embedded Resource`, so the manifest name is
`IconDesk.Icons.Resources.<file>.svg` and Wisej.NET serves it at
`resource.wx/IconDesk.Icons/<file>.svg`. Referencing the pack grows a deployment by one file, and
five screens using one icon still ship a single copy of the artwork.

`AppIcons` is the catalog: a `const string` per icon, an `All` list in gallery order, and a
`Coloured` helper. Callers never type a path, a renamed file breaks the build in one place, and
"which icons do we have?" is an IntelliSense question. The official Wisej-4 packs expose exactly
this shape.

## The twelve icons

| Constant | File | What it means |
|---|---|---|
| `Save` | `save.svg` | commit the current edit |
| `Delete` | `delete.svg` | remove a record |
| `Search` | `search.svg` | open the search field |
| `Filter` | `filter.svg` | narrow a list |
| `Print` | `print.svg` | send to the printer |
| `Export` | `export.svg` | write a file out |
| `Customer` | `customer.svg` | a customer record |
| `Order` | `order.svg` | an order record |
| `Settings` | `settings.svg` | application settings |
| `Success` | `status-success.svg` | a status, not an action |
| `Warning` | `status-warning.svg` | a status, not an action |
| `Error` | `status-error.svg` | a status, not an action |

## Naming

- One concept, one name. `Delete` is the vocabulary; `trash`, `bin` and `remove` are not synonyms
  the catalog will accept.
- The file name matches the constant, lower-cased and hyphenated. A status icon is prefixed
  `status-` so the three of them sort together and read as a set.
- Nothing is named after its appearance. `floppy-o` is what a third-party pack calls the save
  icon; a pack of our own should not make callers remember that.

## Recolouring - the contract every file in this pack keeps

Wisej.NET does not hand the browser a URL for an SVG image source. It reads the file, inlines it
as a `data:image/svg+xml;base64` background, and writes `fill` and `color` onto the root element.
Whether the icon then follows the theme is decided entirely by the artwork:

```xml
<svg viewBox="0 0 24 24" fill="#1f2d3a">
  <path fill="none" stroke="currentColor" stroke-width="1.7" d="…"/>
</svg>
```

- **One meaningful fill on the root** so the file is read as an icon rather than as artwork.
- **`fill="none"` on every shape**, or the injected fill paints the outline solid. An earlier
  version of these files put `fill="none"` on a wrapping `<g>`; the injected root fill won and
  every glyph came back as a blob.
- **`stroke="currentColor"`**, because strokes are never rewritten. A literal stroke colour
  renders perfectly and then ignores every `?color=` suffix it is ever given.

`Assets/status-error-draft.svg`, in the application rather than in the pack, is the Error icon as
it arrived from the designer: `stroke="#8a8f98"` on both shapes. The recolour check draws it, the
suffix does nothing, and pressing **Remove the hard-coded fill** switches to the cleaned
`AppIcons.Error`. That was the one real cleanup this pack needed.

## Colour names that work

`?color=` takes a theme colour name or a literal. Verified on the running page by decoding the
inlined SVG:

| Value | Result |
|---|---|
| `?color=success` | resolved |
| `?color=info` | resolved |
| `?color=invalid` | resolved |
| `?color=highlight` | resolved |
| `?color=#e8a13c` | applied, and identical under every theme |
| `?color=error` | **not** resolved - passed through as a literal `fill="error"` |
| `?color=activeText` | **not** resolved |

A name the theme does not define is a silent no-op. Prefer a name over a literal, because a name
resolves again under the next theme - but test the name.

## Versioning

`AppIcons.Version` is the pack's own version and is shown in the gallery toolbar, so a screenshot
in a bug report says which pack it was taken against.

- Adding an icon is a minor version. Removing or renaming one is a major version, because every
  caller's constant disappears at compile time.
- Changing the artwork behind an existing name is a minor version **and** a note in the release
  notes: browsers cache resource URLs, so a changed picture behind an unchanged URL is a picture
  users will not see until the cache expires.
- A brand mark does not belong in this pack. Recolouring is for neutral glyphs; leave a logo in
  its official colours and record in these notes which icons are deliberately not recolourable.
