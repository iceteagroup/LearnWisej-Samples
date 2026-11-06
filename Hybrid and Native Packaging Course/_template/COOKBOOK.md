# FieldOps cookbook — Wisej.NET Hybrid 4.1.7

Established by building and running the Hybrid & Native Packaging samples on Windows 11 with the
.NET 10.0.303 SDK, MAUI workloads of the 10.0.100 band, `Wisej-4` **4.1.7**, `Wisej-4-Hybrid`
**4.1.7**, `Wisej-4-Hybrid-Native` **4.1.7**, `EmbedIO` 3.5.2, MAUI **9.0.120**. Each entry says how it
was established. The course's verified API list is `scripts/course-plans/hybrid-native-packaging.facts.md`
in the site repository; everything below was checked against the packages themselves.

## Pinned versions

| Project | Packages |
|---|---|
| `FieldOps` (web, `net10.0-windows;net10.0`) | `Wisej-4` 4.1.7, `Wisej-4-Hybrid` 4.1.7 |
| `FieldOps.Client` (MAUI, net9.0 targets) | `Wisej-4-Hybrid-Native` 4.1.7, `EmbedIO` 3.5.2, `Microsoft.Maui.Controls` 9.0.120 |
| `FieldOps.Offline` (Local app, net9.0 targets) | `Wisej-4` 4.1.7, `Wisej-4-Hybrid` 4.1.7, `EmbedIO` 3.5.2 |

`Wisej-4-Hybrid` 4.1.7 depends on `Wisej-4` 4.1.7 exactly; its `net9.0` lib works in a net10.0 web app.
`Wisej-4-Hybrid-Native` 4.1.7 ships `net9.0-android21.0`, `net9.0-ios14.2`, `net9.0-maccatalyst14.0`,
`net9.0-windows10.0.19041.0` only, so the client stays on net9.0 TFMs and **MAUI 9**: the template's
`Version="$(MauiVersion)"` resolves to MAUI 10 under the .NET 10 SDK, so pin `9.0.120` explicitly.

## Creating the projects from the real templates

The VS extension keeps the templates under
`%LOCALAPPDATA%\Microsoft\VisualStudio\18.0_*\Extensions\*\ProjectTemplates\Wisej.NET 4\`
(`CSharp-VS2022-Core-HybridClient`, `-HybridLocal`, `-HybridRemote`). The folders there are stored
**upper-case** (`PLATFORMS\WINDOWS`, `RESOURCES\STYLES`); copy them with the casing the `.vstemplate`
declares (`Platforms\Windows`, `Resources\Styles`), or XAML fails with
`XamlC error XC0124: Resource "Resources/Styles/Colors.xaml" not found`. Replace `$safeprojectname$`,
`$targetframeworks$`, `$applicationurl$` and `4.0.*` versions.

The client template's `App.xaml.cs` sets `MainPage = new HybridShell();`, which MAUI 9 marks obsolete
(CS0618). The samples keep the template form inside `#pragma warning disable CS0618`.

## The build machine's Android workload (machine issue, not a project setting)

On this machine `net9.0-android` and `net10.0-android` restores failed with
`NETSDK1147: ... workloads must be installed: wasm-tools-net9` (resp. `wasm-tools`). Cause: the
`microsoft.net.workload.mono.toolchain.*` manifests 10.0.111 point at Mono runtime packs 10.0.11 /
9.0.17 that are not on disk; the installed packs are 10.0.8 / 9.0.16 (manifests 10.0.108). Without
installing anything: copy the 10.0.108 manifests of `microsoft.net.workload.mono.toolchain.*` and
`microsoft.net.workload.emscripten.*` from `C:\Program Files\dotnet\sdk-manifests\10.0.100\<id>\10.0.108\`
to `<folder>\10.0.300\<id>\` and build with `DOTNETSDK_WORKLOAD_MANIFEST_ROOTS=<folder>`. The proper
fix is `dotnet workload update`/`repair` (not run here). Windows-only builds need no workaround as long
as the project lists no Android target.

Keep Android builds in short paths: under the long scratch path, `aapt2` failed with
`APT2098 ... failed to open file` (Windows path length); the samples folder is short enough.

## Windows client: packaged vs unpackaged

- `dotnet build -f net9.0-windows10.0.19041.0 -p:WindowsPackageType=None` produces
  `bin\Debug\net9.0-windows10.0.19041.0\win10-x64\FieldOps.Client.exe`, which runs directly (the
  Windows App Runtime was already installed). `Device.System.PackagingModel` → `Unpackaged`.
- **The Local app needs a package identity.** `server.WithWisej()` resolves its web root through
  `Windows.Storage.ApplicationData.Current`; in an unpackaged process that throws
  `InvalidOperationException: Operation is not valid due to the current state of the object`
  (stack: `ApplicationData.get_Current` ← `WisejExtensions.GetWebRootPath` ← `WithWisej`), and the
  window shows *"The embedded application could not start: …"* instead of the remote page. The
  unmodified Local template hosted by the unmodified Client template fails the same way (no listener on
  its port). Fix used in FieldOps: register `UseWisejOffline<OfflineStartup>(WisejExtensions.GetLocalUrl())`
  only when `Windows.ApplicationModel.Package.Current` is available; the unpackaged run is Remote only.
  Run the packaged client (VS profile `MsixPackage`, or an installed MSIX) to use the fallback.
- `WisejExtensions` (`GetLocalUrl()`, `WithWisej()`, `Run()`) is in namespace `Wisej.EmbedIO`, assembly
  Wisej.Framework (platform builds of `Wisej-4`). This run's `GetLocalUrl()` returned `http://127.0.0.1:5000`.
- An unpackaged client reports `Device.Versioning.CurrentVersion` from the **assembly version**
  (`[assembly: AssemblyVersion]` in the template's `Properties\AssemblyInfo.cs`), not from
  `ApplicationDisplayVersion`: with the template's `1.0.0.0` the Module 6 check saw `1.0.0`. Keep them in step.
- `dotnet publish -f net9.0-windows10.0.19041.0 -c Release -p:RuntimeIdentifierOverride=win10-x64 -p:AppxPackageSigningEnabled=false`
  produces an unsigned `AppPackages\FieldOps.Client_<ver>_Test\FieldOps.Client_<ver>_x64.msix` with
  `Identity Name` = `ApplicationId` and the manifest's `Publisher`. Installing it needs a trusted signature.

## Server-side Device API, observed

| Fact | How |
|---|---|
| `Device.Valid` is already true in `Program.Main` (the session entry point) of a Hybrid session, false in a browser | server log at sign-in, Windows client vs browser pane |
| Windows facts: `Platform=WinUI Idiom=Desktop IsDesktop=True PackagingModel=Unpackaged`, `Model` = the PC model, `Version` = Windows build, `AppName` = assembly name, `Media.CaptureSupported=True`, `OfflineUrl` empty when no Local app runs | server log |
| `Device.License.LicenseType` = `None` without a key (the client draws *Get a free Hybrid License from Wisej.com* above the page), `Starter` with the owner's key in `config.LicenseKey` (no band) | server log + capture of the client window |
| `Device.TitleBar` is a band the client draws **inside** its window, hidden until `Visible = true`; `Text` alone changes nothing visible. The OS caption stays the client's (assembly name) — `Window.Title` set in `App.CreateWindow` did not change it. | window captures |
| `Device.LocalNotification.Schedule(title, body, DateTime)` called from a server-push handler after `Application.RunInContext(this, …)` | server log |
| `Device.FileSystem.AppDataDirectory` (unpackaged Windows) = `%LOCALAPPDATA%\FieldOps.Client\Data`; `WriteText` there worked from a Remote session | file on disk |
| `Device.Lifecycle` on Windows: minimise → `Deactivated`, `Stopped`; restore → `Resumed`, `Activated`. No `Backgrounding`, no WebView reload on foreground (same session). | server log |
| In a `Page`, `Location` binds to `Control.Location`; write `Wisej.Hybrid.Location` | compiler |
| `Location.CalculateDistance(a, b, DistanceUnits.Kilometers)`, `GeolocationRequest(GeolocationAccuracy, TimeSpan)`, `MapLaunchOptions { Name, NavigationMode }` exist | reflection (XML docs) + compile |

## Wisej.NET (core) facts that mattered

- Server push from a process-wide event: the handler runs outside the session — hop in with
  `Application.RunInContext(page, …)`, then `Application.Update(page)`; never read `Application.Session`
  on the feed thread (capture the technician id at load).
- `Form.ShowDialogAsync()` with buttons that only have `DialogResult` set did **not** close in the
  browser; setting `DialogResult` and calling `Close()` in each button's `Click` did.
- `Application.Navigate(url, "_blank")` in the browser pane refreshed the same tab (same session, no new
  `Program.Main`); a fresh tab gets a new session.
- `Upload` has `HideValue` (not `ShowValue`); `Button.Display` values are `Both/Label/Icon`; `Page` has
  no `Close()` (dispose it after showing the next page).
- `DataGridViewColumn.AllowHtml` renders status chips; `Button.AllowHtml` renders the small DEVICE caption.
- `Image.FromStream` / `new Bitmap(image, size)` (Managed.System.Drawing via `Wisej-4`) downscale a
  4000 × 3000 upload to 1600 × 1200 on net10.0.

## PWA facts (Module 2)

- The browser pane (embedded Chromium) registers service workers and caches, but never fires
  `beforeinstallprompt`; the install path needs Chrome/Edge.
- `.webmanifest` is served as `application/manifest+json` by ASP.NET Core's default provider; set
  `Cache-Control: no-cache` for `sw.js` and the manifest in `StaticFileOptions.OnPrepareResponse`.
- `/` navigations go to the network; only the named shell files are cache-first. `app.wx` traffic
  passes through the worker untouched.

## Verification without a human in the native window

- Server side: log the device facts at sign-in and read them with `preview_logs`.
- Window side: capture **only** the client's own window with `PrintWindow(hwnd, hdc, 2)`
  (PW_RENDERFULLCONTENT) — renders WebView2 content; minimise/restore it with `ShowWindow` to drive
  lifecycle events.
- Drive server push with a loopback-only `POST /dispatch/assign`.
