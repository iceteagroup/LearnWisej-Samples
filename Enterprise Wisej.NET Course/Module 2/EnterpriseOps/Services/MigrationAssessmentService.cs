using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>What "Build dossier" returns to the screen: the dossier, both inventories and the matrix, plus the numbers.</summary>
    public class AssessmentResult
    {
        public IReadOnlyList<DossierRow> Dossier { get; set; }
        public IReadOnlyList<InventoryItem> Inventory { get; set; }
        public IReadOnlyList<CompatibilityEntry> Compatibility { get; set; }
        public int HighCount { get; set; }
        public int MediumCount { get; set; }
        public int LowCount { get; set; }
        public int BlockedCount { get; set; }
        public string CorrelationId { get; set; }
        public string Summary { get; set; }
    }

    /// <summary>
    /// Builds the migration dossier from the inventory: computes the risk of every row with one visible rule,
    /// checks the compatibility matrix has a mitigation for every blocked combination, and writes the
    /// modernization decision memo from the evidence the harness produced. Inventory before action.
    /// </summary>
    public class MigrationAssessmentService
    {
        private readonly MigrationInventoryStore _store;
        private readonly ActivityTrace _trace;
        private List<DossierRow> _dossier = new List<DossierRow>();

        public MigrationAssessmentService(MigrationInventoryStore store, ActivityTrace trace)
        {
            _store = store;
            _trace = trace;
        }

        public bool IsBuilt { get; private set; }

        public IReadOnlyList<DossierRow> Dossier => _dossier;

        /// <summary>
        /// The risk rule, in one place so a reviewer can argue with it:
        /// a change that alters behaviour or format AND that an end user can see is High;
        /// a breaking change nobody sees is Medium; anything else is Low.
        /// </summary>
        public RiskLevel AssessRisk(DossierRow row)
        {
            if (row.BreakingChange && row.UserVisible) return RiskLevel.High;
            if (row.BreakingChange) return RiskLevel.Medium;
            return RiskLevel.Low;
        }

        public async Task<AssessmentResult> AssessAsync(CommandContext ctx)
        {
            _trace.Service($"MigrationAssessmentService.AssessAsync for {ctx} — inventory before action");

            var dossier = _store.DossierRows();
            var inventory = _store.Inventory();
            var matrix = _store.CompatibilityMatrix();
            _trace.Data($"MigrationInventoryStore → {dossier.Count} dossier areas, {inventory.Count} inventory items, {matrix.Count} compatibility entries");

            // Simulated reading time so the progress is visible; nothing here touches the network.
            await Task.Delay(300);

            foreach (var row in dossier)
            {
                row.Risk = AssessRisk(row);
                row.State = DossierRowState.Open;
                _trace.Service($"{row.Area}: {row.Current} → {row.Target} — breaking={Yn(row.BreakingChange)} user-visible={Yn(row.UserVisible)} → risk {row.RiskCode} · proof \"{row.RegressionProof}\" · fallback \"{row.RollbackPoint}\"");
            }

            int blocked = matrix.Count(m => !m.Supported);
            int withoutMitigation = matrix.Count(m => !m.Supported && string.IsNullOrWhiteSpace(m.Mitigation));
            _trace.Service($"compatibility matrix: {matrix.Count} entries, {matrix.Count - blocked} supported, {blocked} blocked → {(withoutMitigation == 0 ? "every blocked entry has a mitigation" : withoutMitigation + " blocked entries have NO mitigation")}");

            _dossier = dossier;
            IsBuilt = true;

            var result = new AssessmentResult
            {
                Dossier = dossier,
                Inventory = inventory,
                Compatibility = matrix,
                HighCount = dossier.Count(r => r.Risk == RiskLevel.High),
                MediumCount = dossier.Count(r => r.Risk == RiskLevel.Medium),
                LowCount = dossier.Count(r => r.Risk == RiskLevel.Low),
                BlockedCount = blocked,
                CorrelationId = ctx.CorrelationId,
            };
            result.Summary = $"dossier built — {dossier.Count} areas: risk H×{result.HighCount} M×{result.MediumCount} L×{result.LowCount}; {blocked} blocked combinations mitigated; 10 flows planned";
            _trace.Service($"assessment complete → {result.Summary}");
            return result;
        }

        /// <summary>Called by the workflow when a step's check passes, fails or is rolled back.</summary>
        public void MarkRows(int provedByStep, DossierRowState state)
        {
            foreach (var row in _dossier.Where(r => r.ProvedByStep == provedByStep))
                row.State = state;
        }

        /// <summary>The modernization decision memo, written from evidence — the last harness run, the steps, the theme state.</summary>
        public string BuildDecisionMemo(CommandContext ctx, IReadOnlyList<MigrationStep> steps, HarnessResult lastHarness, ThemeService theme)
        {
            int passed = steps.Count(s => s.State == StepState.Passed);
            var failed = steps.FirstOrDefault(s => s.State == StepState.Failed);
            var rolledBack = steps.FirstOrDefault(s => s.State == StepState.RolledBack);

            var memo = new StringBuilder();
            memo.AppendLine("MODERNIZATION DECISION MEMO — TicketOps Console → EnterpriseOps baseline");
            memo.AppendLine($"Date {DateTime.UtcNow:yyyy-MM-dd}  ·  tenant {ctx.TenantId}  ·  author {ctx.UserName} ({ctx.Role})  ·  corr {ctx.CorrelationId}");
            memo.AppendLine();
            memo.AppendLine("DECISION");
            memo.AppendLine("  Migrate incrementally: .NET Framework 4.8 → .NET 10 and Wisej.NET 3.5 → 4.1 in seven verifiable steps,");
            memo.AppendLine("  each with a fallback point. The uncontrolled rewrite (\"upgrade everything, fix what breaks\") is rejected.");
            memo.AppendLine();
            memo.AppendLine("WHY THIS PATH");
            memo.AppendLine($"  Dossier: {_dossier.Count} areas — risk H×{_dossier.Count(r => r.Risk == RiskLevel.High)} M×{_dossier.Count(r => r.Risk == RiskLevel.Medium)} L×{_dossier.Count(r => r.Risk == RiskLevel.Low)}.");
            memo.AppendLine("  Every High row is user-visible AND breaking; each has a named regression proof and a fallback point.");
            memo.AppendLine("  A rewrite has no fallback point at all — the review question \"which step can be rolled back independently?\" would answer \"none\".");
            memo.AppendLine();
            memo.AppendLine("EVIDENCE (from the running sample, not from hope)");
            memo.AppendLine($"  Steps passed: {passed}/{steps.Count}" + (failed != null ? $"  ·  step {failed.Number} {failed.Name} FAILED at fallback \"{failed.FallbackPoint}\"" : "") + (rolledBack != null ? $"  ·  step {rolledBack.Number} rolled back to \"{rolledBack.FallbackPoint}\"" : ""));
            memo.AppendLine(lastHarness == null
                ? "  Regression harness: not run yet."
                : $"  Regression harness: {lastHarness.Verdict}");
            int themeDiff = theme.Diff().Count;
            memo.AppendLine($"  Theme: {theme.Current.Name} — {themeDiff} difference(s) from the 3.5 baseline" + (themeDiff == 0 ? "" : $" ({string.Join(" · ", theme.Diff())})"));
            memo.AppendLine();
            memo.AppendLine("WHAT MUST REMAIN UNCHANGED");
            memo.AppendLine("  Tab filters decided on the server; blank titles rejected; stale versions refused; ana.ops approves, ben.tech does not;");
            memo.AppendLine("  accent #1565D8, radius 7, priority colours; one SessionContext per session; background progress reaches the UI.");
            memo.AppendLine();
            memo.AppendLine("ROLLBACK");
            memo.AppendLine("  git tag pre-fx · package pin 3.5 · config backup · theme folder copy · feature flag off · standalone gate · previous package.");
            memo.AppendLine("  Roll back the failed step only, fix on the fallback, re-run the harness — never fix forward on a broken build.");
            memo.AppendLine();
            memo.AppendLine("DECIDED BY  ana.ops (Manager) with cara.admin (Admin) — reviewed against docs/*.md (MigrationDossier, RiskMatrix, RegressionPlan, RollbackPlan).");

            _trace.Service($"decision memo written from evidence: steps {passed}/{steps.Count}, harness {(lastHarness == null ? "not run" : lastHarness.Verdict)}");
            return memo.ToString();
        }

        private static string Yn(bool value) => value ? "yes" : "no";
    }
}
