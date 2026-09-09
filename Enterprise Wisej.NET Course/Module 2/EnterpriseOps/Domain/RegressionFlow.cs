namespace EnterpriseOps.Domain
{
    /// <summary>The kinds of regression the dossier says a migration is "not complete until" it proves.</summary>
    public enum FlowCategory { Security, Behavior, Theme, Session, BackgroundTask }

    public enum FlowOutcome { NotRun, Pass, Fail }

    /// <summary>
    /// One of the ten key flows of the regression harness. A flow protects <b>behavior</b> — the app can
    /// compile and run while a flow fails, and that is exactly the case the harness exists for.
    /// </summary>
    public class RegressionFlow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Screen { get; set; }
        public FlowCategory Category { get; set; }
        public string Expected { get; set; }
        public FlowOutcome Outcome { get; set; } = FlowOutcome.NotRun;
        public string Actual { get; set; } = "";

        public string CategoryText => Category == FlowCategory.BackgroundTask ? "Background task" : Category.ToString();

        public string ResultText => Outcome switch
        {
            FlowOutcome.Pass => "PASS — " + Actual,
            FlowOutcome.Fail => "FAIL — " + Actual,
            _ => "not run"
        };
    }
}
