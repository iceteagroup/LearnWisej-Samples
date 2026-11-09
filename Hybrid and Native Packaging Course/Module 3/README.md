# FieldOps · Hybrid & Native Packaging · Module 3

Lab build for **Module 3 · Desktop Clients for Windows and macOS**. Three projects now:

| Project | What it is |
|---|---|
| `FieldOps/` | The Wisej.NET web app (`Wisej-4` 4.1.7, `Wisej-4-Hybrid` 4.1.7) — the screen of the walkthrough: app bar **FieldOps · Work orders**, **Export report · Open in new window**, the grid Order · Site · Task · Status (WO-4821, WO-4818, WO-4815, WO-4809), the **STATUS** strip and the *N orders · Wisej.NET Hybrid client* footer. |
| `FieldOps.Client/` | The Wisej.NET Hybrid client from the *Wisej.NET 4 Hybrid Client Application* template (`Wisej-4-Hybrid-Native` 4.1.7, MAUI 9.0.120): Windows (`net9.0-windows10.0.19041.0`), plus Mac Catalyst when built on a Mac. |
| `FieldOps.Offline/` | The Local app from the *Wisej.NET 4 Hybrid Local Application* template: `OfflineStartup` (EmbedIO + `WithWisej()`), `OfflineProgram`, `Offline.json/html` — compiling the **same** FieldOps page and service files (linked, not copied). |

## Run it

```bash
# 1. the server
cd "Hybrid and Native Packaging Course/Module 3/FieldOps"
dotnet run -f net10.0 --urls http://localhost:6303
```

```powershell
# 2a. the desktop client, Remote only, no package (quickest):
dotnet build ..\FieldOps.Client\FieldOps.Client.csproj -f net9.0-windows10.0.19041.0 -p:WindowsPackageType=None
..\FieldOps.Client\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\FieldOps.Client.exe

# 2b. the full client with the Local fallback: open FieldOps.slnx in Visual Studio, set
#     FieldOps.Client as startup project, profile "Windows Machine" (MSIX), F5.
#     FieldOps.slnx already marks the client Deploy and builds it as x64, which the MSIX
#     profile needs (otherwise Visual Studio says "The project needs to be deployed before we
#     can debug"); a packaged debug run also needs Windows Developer Mode switched on.

# 3. a new order from dispatch (server push + local notification):
Invoke-RestMethod -Method Post "http://localhost:6303/dispatch/assign?tech=dana"
```

**Licence key.** Copy `FieldOps.Client/HybridLicense.props.example` to `HybridLicense.props` and put
your Wisej.NET Hybrid key in it (or set `WISEJ_HYBRID_LICENSE`). The file is git-ignored; the build
compiles the key into `obj/HybridLicense.g.cs` and `Startup.cs` passes it to `config.LicenseKey`.
Without a key the client still runs, with a *Get a free Hybrid License from Wisej.com* band at the top.

## What to try

| Action | Expected result |
|---|---|
| Open <http://localhost:6303> in a browser | The same list; footer *N orders · browser*. **Export report** downloads `work-orders-yyyy-MM-dd.pdf` (`Application.Download`); **Open in new window** opens `/?order=…` in a new tab. |
| Start the client | The window shows FieldOps; a band **FieldOps — Dana Whitfield** (`Device.TitleBar`) above the app bar; status *Connected · localhost:6303*; footer *… · Wisej.NET Hybrid client*. Server log: `support: … Device.Valid=True Platform=WinUI … PackagingModel=…`. |
| **Export report** in the client | Status *Saved to Documents\FieldOps\work-orders-….pdf*; the file is on the laptop; Windows offers to open it (`Device.Launcher.Open`). A write the OS refuses shows *Export failed: …*, logged on the server. |
| `POST /dispatch/assign` | WO-4830 *Elmtree Pumping Station* appears at the top of every open list (server push); inside the client a Windows notification *New work order*. |
| Select a row, **Open in new window** | A second desktop window titled *FieldOps — WO-4830* with Site, Task, Priority, Assigned, Status. |
| Packaged client, stop the server, wait 5 s | The client switches to FieldOps.Offline: banner *Working offline — FieldOps on this laptop's embedded server*, the orders saved on the laptop. Change a status: *Saved on this device …*. Start the server again and sign in: *Synced 1 change made offline*. |

## Lab tasks → where in the code

| Lab step / deliverable | Implementation |
|---|---|
| Hybrid client project targeting Windows, build block (no optimisation, IL stripping, linking, AOT) | [FieldOps.Client.csproj](FieldOps.Client/FieldOps.Client.csproj) |
| `config.StartupUrl` in `UseWisejHybrid`, DebugMode default (F12) | [Startup.cs](FieldOps.Client/Startup.cs) |
| `Device.TitleBar.Text` after sign-in, behind `Device.Valid` | `SignIn.ApplyTitle` in [SignIn.cs](FieldOps/SignIn.cs), called from the list page's `Load` |
| `btnExportReport_Click`: `Device.FileSystem.MyDocuments` + `WriteBytes`, `lblExportStatus`, `Device.Launcher.Open`; `Application.Download` in a browser | [WorkOrdersPage.cs](FieldOps/WorkOrdersPage.cs); PDF from [ReportService.cs](FieldOps/Services/ReportService.cs) |
| `OrderFeed_NewOrder`: add to the bound list, `Application.Update(this)`, `Device.LocalNotification.Schedule` | `OnFeed` → `Application.RunInContext` → `OrderFeed_NewOrder`; feed in [OrderFeed.cs](FieldOps/Services/OrderFeed.cs); dispatch stand-in `POST /dispatch/assign` in [Startup.cs](FieldOps/Startup.cs) |
| `btnOpenWindow_Click`: `Device.Windows.TryOpenAsync` when `Device.Valid && Device.System.IsDesktop`, else `Application.Navigate(url, "_blank")` | WorkOrdersPage.cs; the window: [WorkOrderWindow.cs](FieldOps/WorkOrderWindow.cs) |
| Offline project: `OfflineStartup` with `WithWisej()`, `UseWisejOffline<OfflineStartup>(WisejExtensions.GetLocalUrl())`, `OfflineTimeout = 5000`, on-screen offline label | [FieldOps.Offline](FieldOps.Offline), Startup.cs, `lblOffline` |
| Support log: `PackagingModel` and `OfflineUrl` at sign-in | `SignIn_Completed` |
| Sync the Local app's data (decision record, Module 1) | [DeviceSync.cs](FieldOps/Services/DeviceSync.cs) |
| MSIX package | `Platforms/Windows/Package.appxmanifest`, [docs/MsixAndDesktopPackaging.md](docs/MsixAndDesktopPackaging.md) |
| Remote vs Local, instances, review answers | [docs/RemoteLocalAndInstances.md](docs/RemoteLocalAndInstances.md) |

## What was verified, and how

- **Builds:** `FieldOps` (net10.0-windows, net10.0) and `FieldOps.Client` + `FieldOps.Offline`
  (`-f net9.0-windows10.0.19041.0`): 0 errors, 0 warnings.
- **Browser** (`fieldops-m3`, port 6303): the list as in the video; `POST /dispatch/assign` put
  WO-4830 at the top of the open browser list through server push; **Open in new window** took the
  `Application.Navigate` path; a fresh tab on `/?order=WO-4830&tech=dana` showed the order window
  (Site, Task, Priority High, *Van 12 · Dana Whitfield*). The browser **Export** click was not made
  (the pane would save a file download).
- **Windows client, unpackaged, against the running server** (read from the server log and from a
  capture of the client's own window only):
  `Device.Valid=True Platform=WinUI Idiom=Desktop IsDesktop=True PackagingModel=Unpackaged OfflineUrl=(not started)`;
  `Device.TitleBar` showed **FieldOps — Dana Whitfield**; `Device.FileSystem.WriteText` saved the
  snapshot (`%LOCALAPPDATA%\FieldOps.Client\Data\FieldOps\orders.json` appeared on disk); a dispatched
  order appeared in the client's grid and the server logged
  `Device.LocalNotification.Schedule "New work order" WO-4830`.
- **Licence:** unlicensed build → band *Get a free Hybrid License from Wisej.com* in the window and
  `Device.License.LicenseType = None`; with the key compiled in → no band, `LicenseType = Starter`.
- **MSIX:** `dotnet publish … -p:AppxPackageSigningEnabled=false` produced
  `FieldOps.Client_1.0.0.1_x64.msix` (44 MB, `Identity Name="com.contoso.fieldops"`,
  `Publisher="CN=Contoso FieldOps Demo"`, `runFullTrust`, `Get-AuthenticodeSignature`: NotSigned).
  It was **not installed** (no certificate change, no sideloading on this machine).
- **Not exercised:** the **Export report** and **Open in new window** clicks inside the native
  window (they need a human click in the client), and therefore `Device.Launcher.Open` and
  `Device.Windows.TryOpenAsync` at run time.
- **Not verified on this machine: the Local fallback.** It needs the packaged client (see below),
  and running the packaged client means registering a package, which this build machine may not do.
  What was proven: the failure's root cause (stack trace), that the unmodified templates fail the same
  way unpackaged, and that the unpackaged client now runs Remote-only instead of showing an error.
  To verify it: F5 with the *Windows Machine* profile (or install the signed MSIX), stop the
  FieldOps server, wait 5 s; `%TEMP%\FieldOps.Offline.log` gets `local app started at http://127.0.0.1:…`
  and the window shows the offline banner.

## Where this sample differs from the lesson

- **The Local fallback requires MSIX on Windows (4.1.7).** First run: the window showed *"The embedded
  application could not start: Operation is not valid due to the current state of the object."* Root
  cause, from a stack trace: `WithWisej()` → `WisejExtensions.GetWebRootPath()` →
  `Windows.Storage.ApplicationData.Current`, which throws without a package identity. The lesson's
  F5 loop is packaged (the template's `MsixPackage` profile); an unpackaged executable cannot host the
  Local app. Fix: `Startup.CanHostLocalApp()` registers `UseWisejOffline` only with a package identity;
  the unpackaged run is Remote only and says so in the support log.
- **`Device.TitleBar` is a band inside the window, not the OS caption.** Setting `Text` alone showed
  nothing; with `Visible = true` the client draws *FieldOps — Dana Whitfield* above the app bar. The
  OS caption stays the client's (*FieldOps.Client* on the unpackaged run). Setting it from
  `Program.Main` also never showed; it is set from the list page's `Load`.
- **The new-order source.** The lab says orders "already arrive through server push" after Module 2;
  Module 2 had no feed, so Module 3 adds `OrderFeed` and a loopback-only `POST /dispatch/assign` as the
  dispatch stand-in.
- **Status names:** the video's *In progress / Complete* are FieldOps' *On site / Completed*.
- **Project names:** the video's `FieldOps.Web` toolbar label is the `FieldOps` project.
- **Report PDF** is produced by a small built-in writer (no third-party library ships with samples).
- **Toast and second window** in the video are drawn on the desktop by Windows; here they were proven
  from the server side, not photographed.
