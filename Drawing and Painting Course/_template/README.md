# `_template` — the VisualOperationsStudio baseline

The working Module 1 application on port **6000**, ready to extend: the `TelemetrySample` model and
`VisualOperationsPage` with its four surfaces (Label + ProgressBar, `Panel.Paint`, `Wisej.Web.Canvas`,
off-screen `Bitmap`).

```bash
cd "Drawing and Painting Course/_template/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6000
```

**[COOKBOOK.md](COOKBOOK.md)** records the framework behaviour, the `Managed.System.Drawing` API
differences and the painting gotchas found while building the course. Read it before changing any
painted surface.
