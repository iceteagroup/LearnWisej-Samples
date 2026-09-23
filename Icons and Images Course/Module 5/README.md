# IconDesk · Icons & Images in Wisej.NET · Module 5

Local lab build for **Module 5 · Embedded Resources and resource.wx**. Five assets - two SVGs, a
PNG, a GIF and one more SVG - are compiled into the assembly and shown through `resource.wx` image
sources. One URL is qualified with the assembly name and the rest are not, which turns out to
decide whether a file dropped beside the application can replace them.
The Module 1-4 pages are unchanged.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 5/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6205
```

Open <http://localhost:6205> and press **Embedded resources**.

## What to try

| Action | Expected result |
|---|---|
| Press **Drop an override beside the app** | `picOk` changes; `picLogo` does not. One file, two controls, and the only difference is the assembly name in the URL. |
| Press **Remove the override** | `picOk` falls back to the embedded resource. No rebuild, no code change. |
| Press **Misspell a resource URL** | Nothing happens. `picWarning` goes blank, no exception is thrown and nothing is logged - the failed request is in the browser network panel. |
| Press **Report the resource URLs** | Each URL is printed with whether it is qualified, and what that means for it. |
| Delete `bin/` and rebuild | The five assets are still there. They are inside `IconDesk.dll`, not beside it. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Five assets marked Embedded Resource | the `EmbeddedResource` item in [IconDesk.csproj](IconDesk/IconDesk.csproj), sources in `Assets/` |
| A page showing all five through `resource.wx` | [ResourceLabPage.Designer.cs](IconDesk/ResourceLabPage.Designer.cs) and [ResourceLabPage.cs](IconDesk/ResourceLabPage.cs) |
| One source qualified with the assembly name, the rest not | `QualifiedLogo` versus the four short forms in `AssignSources` |
| A deployed file that overrides an embedded resource, before and after | `btnOverride_Click` / `btnRemoveOverride_Click` |
| Troubleshooting note for a misspelled URL | [docs/ResourceTroubleshooting.md](IconDesk/docs/ResourceTroubleshooting.md) |

## Notes for anyone extending the resources

- **Qualified URLs cannot be overridden.** `resource.wx/IconDesk/logo.svg` is always the embedded
  resource. `resource.wx/logo.svg` prefers a file of that name in the **application root** and
  falls back to the assembly. Choose the shape on purpose and write down which assets are
  replaceable.
- The override file must be in the application root, named exactly like the resource. A
  `Resources/` subfolder beside the application is **not** searched.
- The manifest name is `<RootNamespace>.<folder>.<file>` and the URL strips
  `<RootNamespace>.Resources.`. This project keeps its sources in `Assets/` and uses
  `LogicalName="IconDesk.Resources.%(Filename)%(Extension)"` so the URL still comes out right while
  leaving `Resources/` free for overrides.
- `Include="Resources**"` is not a valid glob and fails the build with `CS1566`. Use
  `Include="Assets/**/*"`.
- Resource URLs are cached by the browser, which is the point of them. If the bytes behind one
  change, change the URL too - the lab appends `?v=<ticks>`.
- Nothing is transcoded: the GIF arrives as a GIF and the SVGs stay vectors. The SVGs are still
  subject to the icon recolouring from Module 3.
