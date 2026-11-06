# Packaging the FieldOps desktop client

## Two forms for Windows

| | MSIX (packaged) | Unpackaged executable |
|---|---|---|
| How it is built | Visual Studio → right-click **FieldOps.Client** → Publish → *Sideloading* / *Microsoft Store*, or the CLI below | `dotnet build -f net9.0-windows10.0.19041.0 -p:WindowsPackageType=None`, ship the output folder |
| Install / remove | Per user, clean uninstall, Intune or Store can deploy and inventory it | Copy a folder; you own updates, shortcuts and removal |
| Signing | Required to install (Module 6: code-signing certificate, timestamp, subject = manifest publisher) | Optional Authenticode on the .exe |
| `Device.System.PackagingModel` | `Packaged` | `Unpackaged` |
| **Local fallback (`UseWisejOffline`)** | **Works** — the embedded server has the package identity it needs | **Not available** on Windows in 4.1.7 (see below); the client runs Remote only |

FieldOps ships MSIX. The unpackaged form exists for a quick Remote-only run on a developer machine.

## Why the Local app needs MSIX on Windows (verified)

`OfflineStartup.Main` calls `server.WithWisej()`. In Wisej.NET 4.1.7 that resolves the embedded
app's web root through `Windows.Storage.ApplicationData.Current`, which only exists for a process
with a **package identity**. In an unpackaged process it throws:

```
System.InvalidOperationException: Operation is not valid due to the current state of the object.
   at Windows.Storage.ApplicationData.get_Current()
   at Wisej.EmbedIO.WisejExtensions.GetWebRootPath()
   at Wisej.EmbedIO.WisejExtensions.WithWisej[TWebServer](TWebServer server)
   at FieldOps.Offline.OfflineStartup.Main(...)
```

and the client window shows *"The embedded application could not start: Operation is not valid
due to the current state of the object."* instead of FieldOps. The unmodified *Hybrid Local
Application* template hosted by the unmodified *Hybrid Client* template fails the same way when run
unpackaged (no listener on its port), so it is the hosting mode, not FieldOps' code. The template's
own launch profile is `MsixPackage` for this reason.

FieldOps.Client therefore registers `UseWisejOffline<OfflineStartup>(WisejExtensions.GetLocalUrl())`
only when `Windows.ApplicationModel.Package.Current` is available (`Startup.CanHostLocalApp`). An
unpackaged run is Remote only, and the server's support log says so:
`PackagingModel=Unpackaged OfflineUrl=(not started)`.

## Commands

```powershell
# Remote-only developer run (no package, no install)
dotnet build FieldOps.Client\FieldOps.Client.csproj -f net9.0-windows10.0.19041.0 -p:WindowsPackageType=None
.\FieldOps.Client\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\FieldOps.Client.exe

# Full client with the Local fallback: Visual Studio, profile "Windows Machine" (MsixPackage), F5.
# Visual Studio registers a development package (Windows Developer Mode required).

# Unsigned MSIX (for inspection or for signing later - Module 6)
dotnet publish FieldOps.Client\FieldOps.Client.csproj -f net9.0-windows10.0.19041.0 -c Release `
  -p:RuntimeIdentifierOverride=win10-x64 -p:AppxPackageSigningEnabled=false
#   -> FieldOps.Client\bin\Release\net9.0-windows10.0.19041.0\win10-x64\AppPackages\
#      FieldOps.Client_1.0.0.1_Test\FieldOps.Client_1.0.0.1_x64.msix

# Installing it needs a signature the machine trusts (Module 6), then:
#   Add-AppxPackage .\FieldOps.Client_1.0.0.1_x64.msix
```

The package identity comes from `Platforms/Windows/Package.appxmanifest` plus the project:
`Identity Name="com.contoso.fieldops"` (from `ApplicationId`), `Publisher="CN=Contoso FieldOps Demo"`
(must equal the signing certificate's subject), display name *FieldOps*, capability `runFullTrust`.

## The Mac

The same project builds for Mac Catalyst on a Mac only: `net9.0-maccatalyst` is added to the target
frameworks when MSBuild runs on macOS.

```bash
dotnet workload restore FieldOps.Client/FieldOps.Client.csproj
dotnet build FieldOps.Client/FieldOps.Client.csproj -f net9.0-maccatalyst
```

It needs a compatible Xcode, the matching .NET SDK and MAUI workloads, and a signing configuration.
The Wisej.NET designer is not supported on the Mac; F12, the single-instance Mutex and
`PackagingModel` are Windows details — test each desktop feature on the Mac build before promising it.
