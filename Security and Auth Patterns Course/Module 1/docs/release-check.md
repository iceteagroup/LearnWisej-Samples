# Release check: the diagnostics view is not in the packaged build

`IdentityDiagnosticsView`, `IdentityProbe`/`IdentityTrace` and `TestVerifier` live inside `#if DEBUG`.
The admin console's "Identity diagnostics" button is inside `#if DEBUG` too, and Startup registers the
`TestVerifier` and the `IHttpContextAccessor` only in Debug. Nothing is hidden behind a route or a flag:
in Release the code does not exist.

Run on 2026-11-09:

```text
dotnet publish AccessOps/AccessOps.csproj -c Release -o <out>

type name searched in AccessOps.dll   Release publish   Debug build
IdentityDiagnosticsView               0                 1
TestVerifier                          0                 1
IdentityProbe                         0                 1
IdentityTrace                         0                 1
DocumentService                       1                 1
ApplicationPrincipal                  1                 1
```

(`grep -c -a <name> AccessOps.dll` counts the metadata string; 0 means the type is not in the assembly.)

The published Release build was then started (`AccessOps.exe --urls http://localhost:6491`) and opened
in the browser:

- Sign in with a demo account → "Sign-in is not available in this build." (no verifier is registered).
- Admin console → "Access denied." and **no** "Identity diagnostics" button.

How to repeat it: see "Prove the diagnostics view is not packaged" in the module README.
