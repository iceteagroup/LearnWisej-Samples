# CrossPlatform — rendering a PNG on the server, and where that code can run

Lab 6 deliverable. Measured on this project at `http://localhost:6006`, Wisej-4 4.1.0,
`TopologyImageRenderer`, export size 1200 × 700.

## Three names, one thing

| What you see | What it is |
|---|---|
| `Managed.System.Drawing` | the NuGet package you reference (Wisej-4 4.1.0 brings it in) |
| `System.Drawing.Managed.dll` | the assembly on disk |
| `System.Drawing` | the namespace your code uses |

It is a managed rendering engine, not a wrapper over the host's own graphics layer. That is the whole
point: it needs no graphics subsystem underneath it, so it runs on a server that has none.

Microsoft made the classic `System.Drawing.Common` Windows-specific from .NET 6, and the switch that
re-enabled other systems was removed in .NET 7 — which is why the classic package is not an option here.

## The reference graph, as this project actually resolves it

```
Wisej-4 4.1.0
  └── Managed.System.Drawing 4.1.0        compile: lib/net9.0/System.Drawing.Managed.dll
        └── System.Drawing.Common 9.0.0   compile: lib/net9.0/_._      ← deliberately empty
```

`System.Drawing.Common` **is** in the graph, as a transitive dependency of `Managed.System.Drawing` —
and both DLLs end up in `bin/`. But its compile-time assembly is the empty placeholder `_._`, so
nothing in this project ever compiles against it: every `System.Drawing` type resolves to
`System.Drawing.Managed.dll`.

That is the state to preserve. **Do not add a direct `<PackageReference Include="System.Drawing.Common" />`**:
two implementations resolving the same type names is a problem you will not enjoy diagnosing. There is
a quick check in the API itself — the managed `Graphics` has `BeginContainer`/`EndContainer` but **no**
`Save`/`Restore`, and `Pen` has `EndCap` but no `StartCap`. If those members suddenly compile, something
else is supplying the types.

The project multi-targets `net10.0-windows;net10.0`. The `-windows` target exists for the Visual Studio
designer; **the deployed target is plain `net10.0`**, and every run in this module used `-f net10.0`.
Never pin a cross-platform deployment to a Windows-only target framework to make a drawing problem go away.

## The platform matrix

| Target | Font measurement | Drawing |
|---|---|---|
| Modern .NET, Windows | managed | managed |
| Modern .NET, Linux | managed | managed |
| Modern .NET, macOS | managed | managed |
| .NET Framework | managed | falls back to the platform layer |
| Designer build (`net10.0-windows`) | managed | falls back to the platform layer |

For the Wisej.NET hybrid targets — **iOS, Android and macOS Hybrid** — the implication is the same one
the font rule makes: the rendering itself is managed and travels, but anything that assumes a *host*
resource does not. Ship the font, do not borrow it; do not call a Windows-only API from a renderer that
a hybrid client might execute; and keep this renderer free of `Wisej.Web` so it can run wherever the
job that needs the image runs.

## Measured here (Windows 11, .NET 10.0.303, `-f net10.0`, Release-less debug build)

| | Value |
|---|---|
| Output | 1200 × 700 PNG, 60 KB |
| Objects drawn | 10 (5 edges + 5 nodes) |
| First export in a session | **≈ 1850 ms** (JIT plus the font-collection probe) |
| Subsequent exports | **≈ 450 ms** |
| Font resolved | `Microsoft Sans Serif (runtime fallback)` — no font file is shipped in `Fonts/` |
| Controls involved | none: no `using Wisej.Web;` appears in `TopologyImageRenderer.cs` |

Most of that time is PNG encoding of a 0.84-megapixel surface, not drawing.
That is the obvious thing to optimise, and Module 7 does exactly that by exporting a smaller bitmap.

## Size bounds

`Render` clamps each side to 200–2000. Without that, a typo asking for 4000 × 3000 allocates roughly
**48 MB** at 32 bits per pixel before a single shape is drawn — on the server, in a request, for every
session that makes the mistake.

## The degraded paths

| Case | Behaviour |
|---|---|
| Empty scene | A readable image with the title and "No nodes to display", plus the legend — not a blank file. |
| Missing font family | `ResolveFont` falls back to `FontFamily.GenericSansSerif` and records which family it used in `ResolvedFontFamily`. A font file that will not load is caught and ignored. |
| Out-of-range size | Clamped to 200–2000 per side. |
| Anything else thrown | Caught in `btnExport_Click`: the user gets a message in the status line and the topology on screen is unaffected. A failed export must not be a blank space where a picture was expected. |

## Linux evidence — not collected on this machine

Docker Desktop is installed here but its Linux engine was not running, so **the container run was not
executed** and no Linux figures are claimed in this document. What *is* verified is the part that
decides the outcome: the renderer compiles and runs on the plain `net10.0` target, has no `Wisej.Web`
dependency, and takes all of its types from `System.Drawing.Managed.dll`.

To collect the evidence, from the module folder:

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:10.0 dotnet run -f net10.0 --urls http://0.0.0.0:6006
```

Then export from both hosts and compare the two files byte for byte. They will only match if the font
was shipped with the application rather than borrowed from the host — with the fallback in play, the
Windows and Linux images differ because `Microsoft Sans Serif` is not present on the Linux image.
Dropping a licensed `.ttf` into `Fonts/` beside the application is what makes the two outputs identical,
and `ResolvedFontFamily` is how you prove which one was used.

## Why this engine rather than the alternatives

- **ImageSharp** — good, and a different API. It would mean rewriting every renderer in the application
  and keeping two drawing models in one codebase; the on-screen `Paint` handlers must use
  `System.Drawing` regardless, because that is what Wisej.NET hands them.
- **SkiaSharp** — fast, but native binaries per platform, which is exactly the dependency this workload
  is trying not to have.
- **Aspose.Drawing** — a commercial drop-in for the same API; a licensing decision, not a technical one.
- **Microsoft.Maui.Graphics** — aimed at app canvases, not server-side file output.

`System.Drawing.Managed` wins here because the code is already `System.Drawing` — the same geometry
helpers and the same drawing calls serve the painted gauge, the painted grid cells and this exporter.
