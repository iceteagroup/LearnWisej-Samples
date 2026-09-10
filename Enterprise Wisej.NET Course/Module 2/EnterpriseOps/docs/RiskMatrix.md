# Compatibility / risk matrix

Deliverable 2 of the lab. Target frameworks × Wisej.NET versions × third-party packages, decided **before any code
changes**. Every unsupported combination must carry a mitigation, or the migration does not start.

Source of truth in the sample: `Data/MigrationInventoryStore.CompatibilityMatrix()` — shown on the
*Compatibility / risk* tab of `MigrationDossierPage`.

| Component | Current | Target | Supported? | Risk | Mitigation |
|---|---|---|---|---|---|
| Wisej-4 4.1.0 | Wisej-3 3.5 | `net10.0-windows;net10.0` | ✓ supported | **H** | 10 key flows after every step |
| Wisej.NET Designer | VS2019 / 3.5 | VS2022 / 4.1 | ✓ supported | M | open every screen once (`net10.0-windows`) |
| Blue-2019 theme (3.x format) | 3.x json | 4.x engine | **✕ blocked** | **H** | port to a mixin; visual diff per screen |
| JS widget wrappers | 3.x wrapper | 4.x wrapper | ✓ supported | M | re-wire events with `_addListener` |
| Newtonsoft.Json 12 | 12.0.3 | `net10.0` | ✓ supported | L | replace with `System.Text.Json` |
| EntityFramework 6.4 | 6.4.4 | `net10.0` | ✓ supported | L | keep until Module 4 (EF Core) |
| Forms authentication | `System.Web` | Kestrel | **✕ blocked** | **H** | `SessionContext` + `PermissionService`, same rules |
| `Global.asax` startup | `System.Web` | `Program.cs` + `Startup` | **✕ blocked** | M | config backup; session + auth flows |
| IIS hosting | IIS 10 | IIS 10 + ASP.NET Core module | ✓ supported | L | staging deploy; previous package |

**3 blocked combinations, 3 mitigations.** The assessment service asserts exactly that:

```csharp
int blocked            = matrix.Count(m => !m.Supported);
int withoutMitigation  = matrix.Count(m => !m.Supported && string.IsNullOrWhiteSpace(m.Mitigation));
```

and the trace prints `every blocked entry has a mitigation` — or names how many do not. A blocked entry without a
mitigation is the one condition that should stop the project before step 1.

## The three blocked rows, in words

**Blue-2019 theme (3.x format) → 4.x engine.** The most dangerous row in the whole dossier, because it is the one
that *compiles*. The 4.x engine ignores the 3.x theme folder and silently falls back to its own defaults. Nothing
throws. The screens open. Every colour, radius and priority marker is wrong. This is the row the walkthrough's
failure path is built on, and the reason the regression harness contains three theme flows.

**Forms authentication → Kestrel.** `System.Web`'s `FormsAuthentication` does not exist on the target. The
mitigation is not "find the equivalent" — it is "reproduce the *behaviour*": `ana.ops` (Manager) may approve,
`ben.tech` (Technician) may not, and the check happens on the server. Flow 1 asserts both halves.

**`Global.asax` startup → `Program.cs` + `Startup`.** Medium, not High: the change is total but no end user can
see it. If it is wrong, the app does not start — a loud failure, which is the cheap kind.

## Risk levels

| Code | Meaning | Consequence |
|---|---|---|
| **H** | breaking **and** user-visible | needs a named regression proof and its own fallback point; do not batch with other steps |
| **M** | breaking, invisible to users | needs a proof; may share a step |
| **L** | neither | verified by the ordinary build + smoke run |

Risk is derived from two recorded facts, never typed — see `MigrationDossier.md` for the rule.

## Evidence — what the running app shows

- **Build dossier** → the *Compatibility / risk* tab shows the nine entries above with the risk column coloured.
- The trace prints
  `Service:  compatibility matrix: 9 entries, 6 supported, 3 blocked → every blocked entry has a mitigation`.
- The footer of the dossier card summarises it:
  `Dossier: 7 areas · risk H×3 M×3 L×1 · 3 blocked combinations mitigated`.
