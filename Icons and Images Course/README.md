# Icons & Images in Wisej.NET - lab samples

One runnable application per module, built cumulatively. **IconDesk** starts as an empty shell and
ends as a capstone that puts every image mechanism Wisej.NET offers on one screen.

| Module | Folder | Port | What it adds |
|---|---|---|---|
| - | `_template` | 6200 | The empty shell the course starts from |
| 1 | `Module 1` | 6201 | Four icon mechanisms and an `IImage` diagnostics readout |
| 2 | `Module 2` | 6202 | `ImageLab` - four size modes, `LoadAsync`, one SVG at four sizes |
| 3 | `Module 3` | 6203 | `IconGallery` - five image-source kinds, theme switch, `?color=` |
| 4 | `Module 4` | 6204 | Two official icon packs compared, with cache evidence |
| 5 | `Module 5` | 6205 | Embedded resources, `resource.wx`, and a deployment override |
| 6 | `Module 6` | 6206 | `IconDesk.Icons` - the project's own pack and its `AppIcons` catalog |
| 7 | `Module 7` | 6207 | Icon fonts, grid actions, the capstone and the theme QA pass |

Each folder is the complete application at that point in the course, with its own solution.

```bash
cd "Icons and Images Course/Module 1/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6201
```

Every module builds for `net10.0-windows` and `net10.0`. Wisej-4 is pinned to **4.1.4** throughout,
because the official icon packs used from Module 4 depend on it.

## The findings worth knowing before you start

These came out of running the code, and each one contradicts a reasonable first guess.
`_template/COOKBOOK.md` has them with the evidence.

- **`Image` and `ImageSource` are mutually exclusive.** Assigning one clears the other. A lost icon
  is nearly always a second assignment, not a missing file.
- **`LoadAsync` sets `ImageSource`, not `Image`.** The browser fetches the URL; the server never
  downloads or decodes it.
- **Wisej.NET inlines SVG image sources and recolours the ones it reads as icons.** One meaningful
  fill means "icon" and the theme repaints it - even for an absolute URL to a static file.
  Multi-coloured artwork is passed through untouched.
- **Strokes are never rewritten.** A line icon must stroke with `currentColor` or `?color=` does
  nothing to it.
- **A qualified `resource.wx` URL cannot be overridden from disk; an unqualified one can.**
- **Icon fonts do not follow the theme at all.** They are text, and the theme never sees them.
