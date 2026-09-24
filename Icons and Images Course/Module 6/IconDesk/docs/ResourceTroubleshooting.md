# Module 5 lab note - embedded resources, overrides and a misspelled URL

Written from the running ResourceLab page and the browser network panel.

## The five assets

| Tile | Source | Shape |
|---|---|---|
| `picLogo` | `resource.wx/IconDesk/logo.svg` | qualified |
| `picOk` | `resource.wx/status-ok.svg` | unqualified |
| `picWarning` | `resource.wx/status-warning.svg` | unqualified |
| `picPhoto` | `resource.wx/photo.png` | unqualified |
| `picBadge` | `resource.wx/badge.gif` | unqualified |

All five are one `resource.wx` request each on the first view and come from the browser cache
afterwards. Nothing is decoded on the server: the three SVGs are markup the browser draws, not
`System.Drawing.Image` objects.

## How the files get in there

```xml
<EmbeddedResource Include="Assets/**/*" LogicalName="IconDesk.Resources.%(Filename)%(Extension)" />
```

Manifest names are `<RootNamespace>.<folder>.<file>`, and the URL strips
`<RootNamespace>.Resources.`. `LogicalName` is what lets the sources live in `Assets/` while
still being served as `resource.wx/IconDesk/<file>`. `Include="Resources**"` is not a valid glob
and fails the build with `CS1566`.

The sources are kept in `Assets/` rather than `Resources/` deliberately: `Resources/` is a name a
deployment-time override might use, and keeping the two apart means the override cannot overwrite
the original.

## What the two URL shapes really do

| URL | Resolves to |
|---|---|
| `resource.wx/IconDesk/logo.svg` | **always** the embedded resource |
| `resource.wx/status-ok.svg` | a file of that name in the **application root** if present, else the embedded resource |

Established by writing one file and watching both tiles. Pressing **Deploy an override** writes
`status-ok.svg` into the application root. `picOk` changes; `picLogo` does not, and the only
difference between them is the assembly name in the URL.

A `Resources/` subfolder beside the application is **not** searched. The override has to sit in
the application root itself.

The practical rule: qualify anything that must be exactly what you shipped, leave unqualified
anything a deployment is meant to be able to replace, and write down which is which. A customer
re-brands without anyone rebuilding, re-signing or re-releasing the assembly - which also means
that folder now needs the same protection as the binaries, and the override belongs in the
release notes.

## Caching

Resource URLs are cached by the browser, and that is the point: these addresses are stable. It
also means that when the bytes behind one change under your feet, the URL has to change too or
the old picture stays on screen. `Bust()` appends `?v=<ticks>` so the lab's own swap is visible
immediately; a real deployment changes the URL by changing the version in it.

## The misspelling, in six steps

Pressing **Misspell one URL** points `picWarning` at `resource.wx/IconDesk/status-warnning.svg` -
one doubled letter. There is no exception, no log entry and no compiler error. The control simply
shows nothing.

The checklist that finds it, in this order:

1. **Build Action** is `Embedded Resource` - yes, the `.csproj` globs the whole folder.
2. **Extension** is on the served list - `.svg` is.
3. **Assembly loaded** - `IconDesk.dll` is in the process.
4. **Path segments** - `status-warnning.svg` ✕. **This is the one.**
5. **Manifest name** - unchanged, the namespace is fine.
6. **Request the URL** - 404 confirmed in the browser.

Step 4 found it without anyone having to guess a long manifest resource name.

The tile does not stay blank. `AssetTile` has a `Missing` state that draws a dashed **404** box,
turns the file name red and says so in `lblResourceReport`, because a silent empty rectangle is
something a customer finds before you do.

## Where this sample differs from the lesson video

The video overrides **`logo.svg`** from a `Resources\` folder and shows the qualified URL
`resource.wx/IconDesk/logo.svg` serving the deployed file. That cannot happen: a qualified URL
always resolves to the embedded resource, and a `Resources/` subfolder is not searched at all.
The sample overrides the unqualified `status-ok.svg` from the application root instead, and the
status strip says in one line what changed and what did not - which is the finding the module is
actually about.
