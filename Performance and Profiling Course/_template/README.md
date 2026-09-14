# `_template`

This folder holds the course cookbook — [`COOKBOOK.md`](COOKBOOK.md) — and nothing else.

The other courses in this repository keep a skeleton project here, because each of their modules was
built from one. This course was not built that way: **`Module 1` is the scaffold**, and every later
module folder is a copy of the previous one plus that module's single change. Shipping a separate
skeleton would mean shipping a project that nothing was ever built from.

So, to write a new sample for this course:

1. Read [`COOKBOOK.md`](COOKBOOK.md) — the conventions, the verified Wisej.NET and EF Core facts, and
   the numbers each module measured.
2. Copy the folder of the module before yours (source only: `bin`, `obj`, `App_Data` and `exports` are
   all rebuilt).
3. Change the port in `Properties/launchSettings.json`, the `<title>` in `Default.html`, the module
   line in `MainPage.Designer.cs` and the comment in the `.csproj`.
4. Make **one** change, measure it against the folder you copied, and write the before/after with the
   medians of three runs and the spread.
