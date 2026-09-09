using System.Reflection;

// GenerateAssemblyInfo is off in the csproj, so the version the diagnostics page shows comes from here.
// The informational version is what a support engineer needs: the semantic version plus the commit it was
// built from. DiagnosticsService.Version reads it back through reflection — no hand-typed version string.
[assembly: AssemblyTitle("EnterpriseOps Command Center")]
[assembly: AssemblyProduct("EnterpriseOps")]
[assembly: AssemblyVersion("2.4.1.0")]
[assembly: AssemblyFileVersion("2.4.1.0")]
[assembly: AssemblyInformationalVersion("2.4.1+sha.9e21b0")]
