# Local development package

Wisej-4.4.1.4.nupkg was repacked from the updated extracted Wisej-4 4.1.4 NuGet cache on 2026-09-27. It contains only lib/net8.0 (the framework DLL and XML documentation). Other framework binaries and platform-specific dependency groups are excluded; plain .NET 8/9/10 dependency groups, build props, license and package metadata are retained.

This is an unsigned development package. The original signature was removed because the payload changed. Do not publish this replacement as an official NuGet release.

NuGet.Config maps Wisej-4 exclusively to this directory. Other dependencies use nuget.org. Keep the package and configuration together so a clean checkout can restore them. Because this replacement keeps version 4.1.4, use a fresh restore cache when verifying the updated binaries.