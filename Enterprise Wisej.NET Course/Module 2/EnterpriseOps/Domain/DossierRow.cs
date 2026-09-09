namespace EnterpriseOps.Domain
{
    /// <summary>Risk as the dossier prints it: H / M / L.</summary>
    public enum RiskLevel { Low, Medium, High }

    /// <summary>Where a dossier row stands in the migration.</summary>
    public enum DossierRowState
    {
        /// <summary>Planned; no proof yet.</summary>
        Open,
        /// <summary>The regression proof ran and passed — the row closes.</summary>
        Proven,
        /// <summary>The regression proof ran and failed — the fallback point is the next move.</summary>
        Failed,
        /// <summary>Restored to the fallback point; waiting for the fix and a re-run.</summary>
        RolledBack
    }

    /// <summary>
    /// One row of the migration dossier (docs/migration/MigrationDossier.md):
    /// what changes, what it risks, how it is proven and how to get back.
    /// </summary>
    public class DossierRow
    {
        public string Area { get; set; }             // "Target framework", "Wisej.NET version", …
        public string Current { get; set; }          // ".NET Framework 4.8"
        public string Target { get; set; }           // ".NET 10"
        public string RegressionProof { get; set; }  // "full build + smoke run"
        public string RollbackPoint { get; set; }    // "git tag pre-fx"

        /// <summary>Input to the risk rule: does the change alter runtime behaviour or format?</summary>
        public bool BreakingChange { get; set; }

        /// <summary>Input to the risk rule: can an end user see the change on a screen?</summary>
        public bool UserVisible { get; set; }

        /// <summary>Computed by MigrationAssessmentService — never typed by hand.</summary>
        public RiskLevel Risk { get; set; }

        /// <summary>The migration step (1–7) whose check proves this row.</summary>
        public int ProvedByStep { get; set; }

        public DossierRowState State { get; set; } = DossierRowState.Open;

        public string RiskCode => Risk switch { RiskLevel.High => "H", RiskLevel.Medium => "M", _ => "L" };

        public string StateText => State switch
        {
            DossierRowState.Proven => "✓ proven",
            DossierRowState.Failed => "✕ failed",
            DossierRowState.RolledBack => "↶ rolled back",
            _ => "open"
        };
    }
}
