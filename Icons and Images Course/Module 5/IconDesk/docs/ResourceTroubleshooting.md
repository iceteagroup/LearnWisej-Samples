# Module 5 lab note - embedded resources, overrides and a misspelled URL

Five assets are compiled into `IconDesk.dll`:

```xml
<EmbeddedResource Include="Assets/**/*" LogicalName="IconDesk.Resources.%(Filename)%(Extension)" />
```

which produces exactly these manifest names, confirmed by reflecting over the built assembly:

```
IconDesk.Resources.logo.svg
IconDesk.Resources.status-ok.svg
IconDesk.Resources.status-warning.svg
IconDesk.Resources.photo.png
IconDesk.Resources.badge.gif
```

The source files sit in `Assets/`, not `Resources/`, and `LogicalName` maps them onto the
`IconDesk.Resources.` prefix. That is deliberate: `Resources/` is a perfectly ordinary folder name
that a deployment might also use, and keeping the compiled originals somewhere else means the lab's
override cannot overwrite them. Without `LogicalName` the folder name has to be `Resources` for the
URL to come out right, because the build strips `<RootNamespace>.Resources.` and nothing else.

## Two URL shapes, and the difference is not cosmetic

| URL | Resolves to |
|---|---|
| `resource.wx/IconDesk/logo.svg` | **always** the embedded resource |
| `resource.wx/logo.svg` | a file named `logo.svg` in the application root if one exists, otherwise the embedded resource |

Both shapes work - the short form is not a convenience that only resolves sometimes. What differs
is whether anything on disk can get in front of it.

This was established by fetching both URLs from the running application while moving one file
around:

- With no file on disk, both URLs return the embedded original.
- With a replacement at the **application root**, `resource.wx/logo.svg` returns the replacement
  and `resource.wx/IconDesk/logo.svg` still returns the original.
- With a replacement in a `Resources/` **subfolder**, neither URL changes. The subfolder is not
  searched; the file has to sit in the application root, named exactly like the resource.

## The override, live

**Drop an override beside the app** writes `status-ok.svg` into the application root. Nothing is
rebuilt, nothing in the assembly is touched, and the result on screen is:

- `picOk` - `resource.wx/status-ok.svg` - **changes**.
- `picLogo` - `resource.wx/IconDesk/logo.svg` - does not.

One file, two controls, opposite outcomes, and the only difference between them is the assembly
name in the URL.

**Remove the override** deletes the file and `picOk` falls back to the embedded resource with no
code change.

### Choose the shape on purpose

- **Qualify** anything that must be exactly what you shipped: a product mark, a legal notice, a
  status glyph whose colour carries meaning. Qualification is a guarantee, and it also disambiguates
  when several loaded assemblies each embed a `logo.svg`.
- **Leave unqualified** anything a deployment is allowed to replace: a customer's logo, a
  site-specific banner. It turns "we need a rebuild for their branding" into "drop a file next to
  the executable".

Write down which is which. An asset that is unqualified by accident is an asset a stray file in the
deployment folder can silently replace.

### One thing the override needs

The browser caches these URLs, correctly - stable addresses are the point. When the bytes behind one
change under your feet, the URL has to change too or the old picture stays on screen. The lab adds a
`?v=<ticks>` parameter for exactly that reason. In production this matters at deployment time: if
you replace an override file, either rename it or give the URL a version parameter, or returning
users keep the old picture until their cache expires.

## What a misspelled resource URL looks like

**Misspell a resource URL** points `picWarning` at `resource.wx/IconDesk/status-warnning.svg`.

What happens: **nothing**. No exception, no log entry, no server-side error. The control renders
empty and the application carries on. That is the right behaviour for a web framework - one missing
icon should not take a page down - but it does mean the failure is invisible from C#.

The steps that actually find it, in the order worth trying:

1. **Open the browser network panel and reload.** The failed request for the resource is there,
   with the exact URL that was asked for. Nine times out of ten the typo is visible immediately.
2. **Compare against the manifest name.** Reflect over the built assembly:
   `Assembly.GetExecutingAssembly().GetManifestResourceNames()`. If the name you expect is not in
   that list, the problem is the build, not the URL - most often a file added to the project as
   `Content` or `None` instead of `EmbeddedResource`, which is silent in every other respect.
3. **Check the prefix.** `IconDesk.Resources.logo.svg` becomes `resource.wx/IconDesk/logo.svg`. A
   file in a differently named folder becomes `IconDesk.Assets.logo.svg` and will not resolve -
   which is what `LogicalName` in the `.csproj` exists to control.
4. **Try the unqualified form.** If `resource.wx/logo.svg` works and
   `resource.wx/IconDesk/logo.svg` does not, the assembly name in the URL is wrong - it is the
   **assembly** name, not the namespace and not the project folder.
5. **Only then look at the control.** A blank control with a correct URL is usually a second image
   property assignment, which is the Module 1 finding.

## Formats

All five common cases are covered on purpose: two SVGs, one PNG, one GIF and one more SVG. All four
formats travel as-is - Wisej.NET does not transcode an embedded resource, so the GIF arrives as a
GIF and the SVG stays a vector. The SVGs are still subject to the icon recolouring described in
Module 3's note: `status-ok.svg` has a single meaningful fill and takes the theme's colour, while
`status-warning.svg` has two and keeps its amber.
