# Signing the FieldOps Hybrid client packages

A signature proves two things only: the bytes are the bytes that were signed, and a named identity
signed them. Every package is published from the **FieldOps.Client** project (not the solution), in
**Release**, with the no-trimming / no-linking / no-AOT block still in force (it is unconditional in
`FieldOps.Client.csproj`).

## Release build settings

| Setting | Where | Value |
|---|---|---|
| Production address | `FieldOpsServer.Url` (Release) | `https://fieldops.example.com/` |
| Licence key | `config.LicenseKey` in `Startup.cs` | from `HybridLicense.props` / `WISEJ_HYBRID_LICENSE`, never committed |
| Client log | `config.LogUrl` | `<server>/client-log` |
| Version | `ApplicationDisplayVersion` / `ApplicationVersion` (+ `AssemblyVersion` for unpackaged Windows) | `2.6.0` / `260` |
| Icon | `<MauiIcon Include="Resources\AppIcon\appicon.svg" ForegroundFile="Resources\AppIcon\appiconfg.svg" />` | |
| Custom splash | `<HybridSplashScreen Include="Resources\Splash\splash.svg" BaseSize="456,456" />` | commented out: needs a paid Hybrid plan |

## Per platform

| Platform | Identity | Configured in | Command | Verify like the device does |
|---|---|---|---|---|
| **Windows (MSIX)** | Code-signing certificate; subject = `Publisher="CN=Contoso FieldOps Demo"` in `Package.appxmanifest`; RFC 3161 timestamp | `FieldOpsCertificateThumbprint` (Signing.props) → `PackageCertificateThumbprint`, `AppxPackageSigningTimestampServerUrl` | `dotnet publish -f net9.0-windows10.0.19041.0 -c Release -p:RuntimeIdentifierOverride=win10-x64` | `Get-AuthenticodeSignature FieldOps.Client_2.6.0.0_x64.msix` → `Valid`, timestamp present; install on a clean VM through Intune |
| **Android (.aab)** | Upload key enrolled in **Play App Signing** (Google re-signs with the app signing key) | `FieldOpsUploadKeystore`, `FieldOpsUploadKeyAlias`; passwords `env:FIELDOPS_UPLOAD_STORE_PASS`, `env:FIELDOPS_UPLOAD_KEY_PASS` | `dotnet publish -f net9.0-android -c Release` | `jarsigner -verify -verbose -certs <file>.aab`; Play Console shows the upload certificate fingerprint |
| **iOS / Mac Catalyst** | Apple Distribution certificate + provisioning profile (App ID `com.contoso.fieldops`, entitlements, custom-app distribution) | `FieldOpsAppleCodesignKey`, `FieldOpsAppleProvisioningProfile` → `CodesignKey`, `CodesignProvision` | on the Mac: `dotnet publish -f net9.0-ios -c Release` / `-f net9.0-maccatalyst` | `codesign -dv --verbose=4 FieldOps.app`; Apple Business Manager accepts the build |

`Signing.props` (git-ignored) holds the non-secret identifiers on the signing machine; passwords stay
in environment variables / the CI secret store. Copy `FieldOps.Client/Signing.props.example`.

## Key custody (fill in per organisation)

| Key | Holder | Where it lives | Expires | If lost |
|---|---|---|---|---|
| Windows code-signing certificate | Release manager | HSM / Azure Key Vault, thumbprint in `Signing.props` | per certificate (typically 1–3 years) | revoke, buy a new one with the **same subject**, re-sign |
| Android upload key | Release manager | offline keystore + backup | 25+ years | Google resets the upload key (the app signing key is held by Google) |
| Apple distribution certificate | Apple developer account admin | Mac keychain of the build agent | 1 year | revoke and recreate in the developer portal; regenerate profiles |
| Apple provisioning profile | Apple developer account admin | build agent | 1 year | regenerate |

## What happened on the build machine

- **Windows:** `dotnet publish … -p:AppxPackageSigningEnabled=false` produced an **unsigned** MSIX (no
  signing certificate exists here and none was created or installed). Module 3's unsigned package was
  inspected with `Get-AuthenticodeSignature` (`NotSigned`).
- **Android:** see the Module 6 README for the Release publish result; without `Signing.props` the
  upload-key block is skipped, so the bundle is not upload-key signed.
- **iOS / Mac Catalyst:** not built — needs a Mac with Xcode.
- No signing identity was created, imported or trusted on this machine.
