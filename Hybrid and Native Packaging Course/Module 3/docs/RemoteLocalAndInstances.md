# Remote, Local, both — and more than one window

## What FieldOps.Client does

```csharp
if (CanHostLocalApp())                                   // packaged (MSIX) on Windows
    builder.UseWisejOffline<OfflineStartup>(WisejExtensions.GetLocalUrl());
builder.UseWisejHybrid(config =>
{
    config.StartupUrl = ServerUrl;                       // Remote first: http://localhost:6303/ in dev
    config.OfflineTimeout = 5000;                        // Local app 5 s after the connection is lost
});
```

| Situation | What runs | What the technician sees |
|---|---|---|
| Server reachable | Remote FieldOps, handlers on the server | *Connected · server* in the status strip; every server deploy is live on the next load |
| Connection lost < 5 s | Still the remote page, waiting | Wisej's reconnect behaviour |
| Connection lost ≥ 5 s, packaged client | **FieldOps.Offline** on the embedded EmbedIO server | Orange banner *Working offline — FieldOps on this laptop's embedded server*, status *Working offline — N orders saved on this laptop*, footer *Local app on this device* |
| A status change in the Local app | Saved in the Local app's store **and** recorded in `pending.json` on the device | Amber *Saved on this device … reaches the FieldOps server when you are back online* — never "saved" |
| Next Remote sign-in | `DeviceSync.ReplayPending` sends each change with the version it was based on | *Synced 1 change made offline* — or which ones the server refused because the order moved on |

The device files live under `Device.FileSystem.AppDataDirectory\FieldOps\` (`orders.json`,
`pending.json`). On the unpackaged Windows run of this lab that resolved to
`%LOCALAPPDATA%\FieldOps.Client\Data\FieldOps\orders.json`.

## Two instances on one laptop, network down

- **Without `WisejExtensions.GetLocalUrl()`** both instances start an embedded server on
  `http://localhost:5000`; the second cannot bind the port and its Local app never starts.
- **With it** each instance gets the next free port (this lab's run received
  `http://127.0.0.1:5000`), so each has its own embedded FieldOps.
- **Choose a single-instance Mutex instead** when the depot wants exactly one FieldOps per laptop
  (one technician, one device store, no two windows racing on `pending.json`). FieldOps carries it in
  `Platforms/Windows/App.xaml.cs`, switched on with `-p:DefineConstants=FIELDOPS_SINGLE_INSTANCE`.

## Second window

`btnOpenWindow_Click`: `Device.Valid && Device.System.IsDesktop` →
`Device.Windows.TryOpenAsync(url, "FieldOps — WO-4830")`; otherwise `Application.Navigate(url, "_blank")`.
The URL is `/?order=WO-4830&tech=dana`, which `Program.Main` answers with `WorkOrderWindow` — a new
session, so the second window is independent of the first. Multiple windows are a desktop feature
only.

## Review answers

- **Report on the server's disk:** the export used `System.IO.File`. In a Remote app the handler
  runs on the web server, so `System.IO` writes there. `Device.FileSystem.WriteBytes` sends the
  bytes over the bridge and the client writes them on the laptop.
- **Unpackaged instead of MSIX:** IT gains xcopy deployment and no signing step; it loses clean
  install/uninstall, Intune/Store distribution and inventory, and — on Windows with Hybrid 4.1.7 —
  the Local fallback. The support log tells them apart: `PackagingModel=Packaged` vs `Unpackaged`
  (and `OfflineUrl` started or not).
