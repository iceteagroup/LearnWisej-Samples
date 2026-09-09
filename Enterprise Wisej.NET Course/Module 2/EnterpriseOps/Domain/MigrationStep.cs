namespace EnterpriseOps.Domain
{
    public enum StepState { Pending, Running, Passed, Failed, RolledBack }

    /// <summary>
    /// One of the seven verifiable steps of the incremental path:
    /// Compile → Run → Compare behavior → Check themes → Verify sessions → Review deployment → Measure performance.
    /// Every step names the check that proves it and the fallback point that undoes it.
    /// </summary>
    public class MigrationStep
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Check { get; set; }          // what proves the step
        public string FallbackPoint { get; set; }  // how to get back if the check fails
        public StepState State { get; set; } = StepState.Pending;
        public string Detail { get; set; } = "";   // last outcome, one line

        public string StateGlyph => State switch
        {
            StepState.Passed => "✓",
            StepState.Failed => "✕",
            StepState.Running => "▶",
            StepState.RolledBack => "↶",
            _ => "·"
        };
    }
}
