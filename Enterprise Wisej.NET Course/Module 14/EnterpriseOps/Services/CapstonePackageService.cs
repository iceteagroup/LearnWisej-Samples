using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>The outcome of one capstone-package verification: the command result plus every line behind it.</summary>
    public sealed class CapstoneVerification
    {
        public CommandResult Result { get; set; }
        public List<PackageCheck> Checks { get; set; } = new List<PackageCheck>();
        public int Passed => Checks.Count(c => c.Passed);
        public int RequiredFailed => Checks.Count(c => c.Required && !c.Passed);
    }

    /// <summary>
    /// Verifies the capstone package the lab asks the student to submit. Each deliverable is one check:
    /// the file has to exist in <c>docs/</c>, carry content, and contain the section that makes it that
    /// deliverable rather than an empty heading — a demo script without steps is not a demo script.
    ///
    /// The point of automating it: the reviewer sees the package pass or fail in front of them, with the
    /// evidence (path, size, missing section) next to each line, instead of taking a checklist on trust.
    /// </summary>
    public sealed class CapstonePackageService
    {
        private readonly DocsFolder _docs;
        private readonly DocumentationIndexService _index;
        private readonly PermissionService _permissions;
        private readonly AuditLog _audit;
        private readonly ActivityTrace _trace;

        public CapstonePackageService(DocsFolder docs, DocumentationIndexService index, PermissionService permissions, AuditLog audit, ActivityTrace trace)
        {
            _docs = docs;
            _index = index;
            _permissions = permissions;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>Deliverable → file → the marker that proves the file is really that deliverable.</summary>
        private static readonly Deliverable[] Deliverables =
        {
            new Deliverable("AI prompt library", "docs/PromptLibrary.md", "## Prompt header", true),
            new Deliverable("Generated-code review checklist", "docs/GeneratedCodeReviewChecklist.md", "| Q1 |", true),
            new Deliverable("Project documentation index (Markdown)", "docs/DocumentationIndex.md", "| Document |", true),
            new Deliverable("Project documentation index (MCP resources)", "docs/index.json", "\"resources\"", true),
            new Deliverable("Capstone demo script", "docs/CapstoneDemoScript.md", "## Stop 1", true),
            new Deliverable("Production readiness statement", "docs/ProductionReadiness.md", "## Sign-off", true),
            new Deliverable("AI-assisted review patterns", "docs/AIAssistedReviewPatterns.md", "## Prompt header", false),
            new Deliverable("AI usage notes (accepted / rejected)", "docs/AIUsageNotes.md", "PR #214", false),
            new Deliverable("Architecture defense deck outline", "docs/DefenseDeckOutline.md", "## Slide 1", false),
            new Deliverable("Capstone architecture diagram", "docs/capstone-package.svg", "<svg", false),
        };

        /// <summary>Set by the failure-path button: pretend one deliverable was never written.</summary>
        public bool SimulateMissingDeliverable { get; set; }

        /// <summary>
        /// Runs the checks. <paramref name="quiet"/> is used by the diagnostics probe, which has already
        /// authorised the caller and does not want ten more trace lines.
        /// </summary>
        public List<PackageCheck> Verify(CommandContext ctx, bool quiet = false)
        {
            var checks = new List<PackageCheck>();

            if (!_docs.Found)
            {
                checks.Add(new PackageCheck
                {
                    Deliverable = "docs/ folder",
                    Path = "docs/",
                    Passed = false,
                    Required = true,
                    Detail = "not found next to the running project — run from the project folder",
                });
                return checks;
            }

            foreach (Deliverable deliverable in Deliverables)
                checks.Add(Check(deliverable));

            // The index has to list the deliverables, not just exist: a documentation index that does not
            // point at the package is the classic "documented, but not findable" failure.
            List<DocEntry> indexed = _index.Load();
            int missingFromIndex = Deliverables.Count(d => d.Required && !indexed.Any(e => string.Equals(e.Path, d.Path, StringComparison.OrdinalIgnoreCase)));
            checks.Add(new PackageCheck
            {
                Deliverable = "Every required deliverable is listed in the index",
                Path = DocumentationIndexService.IndexPath,
                Required = true,
                Passed = indexed.Count > 0 && missingFromIndex == 0,
                Detail = indexed.Count == 0
                    ? "the index is empty"
                    : missingFromIndex == 0
                        ? $"{indexed.Count} resources indexed, every required deliverable present"
                        : $"{missingFromIndex} required deliverable(s) missing from the index",
            });

            // Every indexed document must resolve on disk — the MCP endpoint would 404 otherwise.
            int broken = indexed.Count(e => !e.Exists);
            checks.Add(new PackageCheck
            {
                Deliverable = "Every indexed path resolves on disk",
                Path = DocumentationIndexService.IndexPath,
                Required = true,
                Passed = indexed.Count > 0 && broken == 0,
                Detail = broken == 0 ? $"{indexed.Count}/{indexed.Count} resolve" : $"{broken} of {indexed.Count} would answer 404: " +
                         string.Join(", ", indexed.Where(e => !e.Exists).Select(e => e.Path)),
            });

            if (SimulateMissingDeliverable)
            {
                checks.Add(new PackageCheck
                {
                    Deliverable = "Security review sign-off",
                    Path = "docs/SecurityReviewSignOff.md",
                    Required = true,
                    Passed = false,
                    Detail = "not written yet — listed as an open item in the readiness statement, owner ana.ops",
                });
            }

            if (!quiet)
            {
                foreach (PackageCheck check in checks)
                    _trace.Docs($"{(check.Passed ? "ok  " : "MISS")} {check.Deliverable} — {check.Path} · {check.Detail}");
            }

            return checks;
        }

        /// <summary>
        /// The screen-facing command: authorise, verify, audit, return a typed result. The handler only asks
        /// the question — everything that decides an answer is in here.
        /// </summary>
        public async Task<CapstoneVerification> VerifyAsync(CommandContext ctx)
        {
            var verification = new CapstoneVerification();

            string refusal = _permissions.Check(ctx.User, Permission.VerifyCapstonePackage);
            if (refusal != null)
            {
                _audit.Write(ctx.TenantId, ctx.UserName, "VerifyCapstonePackage", "docs/", false, refusal, ctx.CorrelationId);
                _trace.Security($"VerifyCapstonePackage denied — {refusal}");
                verification.Result = CommandResult.Fail(ctx.CorrelationId, refusal);
                return verification;
            }

            _trace.Service($"verifying the capstone package in {(_docs.Found ? _docs.Root : "(docs folder not found)")}");
            await Task.Delay(120).ConfigureAwait(true);

            verification.Checks = Verify(ctx);
            int failed = verification.RequiredFailed;

            _audit.Write(ctx.TenantId, ctx.UserName, "VerifyCapstonePackage", "docs/", failed == 0,
                $"{verification.Passed}/{verification.Checks.Count} checks pass", ctx.CorrelationId);

            verification.Result = failed == 0
                ? CommandResult.Ok(ctx.CorrelationId)
                : CommandResult.Fail(ctx.CorrelationId, $"{failed} required deliverable(s) incomplete — the package is not ready to submit.");

            _trace.Service($"capstone package: {verification.Passed}/{verification.Checks.Count} checks pass, {failed} required missing");
            return verification;
        }

        private PackageCheck Check(Deliverable deliverable)
        {
            string content = _docs.ReadAllText(deliverable.Path);
            long bytes = _docs.SizeOf(deliverable.Path);

            if (content == null)
                return new PackageCheck { Deliverable = deliverable.Name, Path = deliverable.Path, Required = deliverable.Required, Passed = false, Detail = "file not found" };
            if (bytes < 200)
                return new PackageCheck { Deliverable = deliverable.Name, Path = deliverable.Path, Required = deliverable.Required, Passed = false, Detail = $"only {bytes} bytes — a placeholder, not a deliverable" };
            if (content.IndexOf(deliverable.Marker, StringComparison.OrdinalIgnoreCase) < 0)
                return new PackageCheck { Deliverable = deliverable.Name, Path = deliverable.Path, Required = deliverable.Required, Passed = false, Detail = $"missing the \"{deliverable.Marker}\" section" };

            return new PackageCheck { Deliverable = deliverable.Name, Path = deliverable.Path, Required = deliverable.Required, Passed = true, Detail = $"{bytes:n0} bytes, \"{deliverable.Marker}\" present" };
        }

        private sealed class Deliverable
        {
            public Deliverable(string name, string path, string marker, bool required)
            {
                Name = name;
                Path = path;
                Marker = marker;
                Required = required;
            }

            public string Name { get; }
            public string Path { get; }
            public string Marker { get; }
            public bool Required { get; }
        }
    }
}
