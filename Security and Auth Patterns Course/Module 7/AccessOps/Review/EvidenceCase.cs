#if DEBUG
namespace AccessOps.Review
{
    /// <summary>How a row of the evidence matrix was produced (the video's "Evidence" column).</summary>
    public enum EvidenceLabel
    {
        /// <summary>Decided by the running AccessOps on the shared store: a real HTTP request to a running route, or a principal produced by that route's real sign-in code.</summary>
        Live,

        /// <summary>The policy run in-process with a constructed principal or a software authenticator (the PolicyHarness rules); runs on any machine.</summary>
        Harness,

        /// <summary>Needs IIS, the domain, a live provider or a real authenticator; ran here on a fixture or stand-in, with an exact rerun plan.</summary>
        Fixture,

        /// <summary>Second node or restart, run as a recorded exercise.</summary>
        Simulated
    }

    /// <summary>
    /// One row of the 12-case evidence matrix: the fixed definition from the capstone brief and the
    /// walkthrough (number, title, route, fixture, expected outcome, label, rerun plan) and what the
    /// run filled in (alias, actual result, outcome chip, denying boundary, repository calls,
    /// correlation label). No row ever holds a session ID, token, handle, password or SID.
    /// </summary>
    public sealed class EvidenceCase
    {
        /// <summary>1-12.</summary>
        public int Number { get; init; }

        /// <summary>The case as the review lists it ("Tenant A requests tenant B record 17").</summary>
        public string Title { get; init; }

        /// <summary>"Intranet" or "Partner".</summary>
        public string Route { get; init; }

        /// <summary>The reserved fixture IDs from labs/fixtures.tsv ("F02"), or "—".</summary>
        public string Fixture { get; init; }

        /// <summary>The expected outcome in words.</summary>
        public string Expected { get; init; }

        /// <summary>The outcome chip the case must end with (Allow, Deny, Reverted, Rotated, One grant, Safe deny).</summary>
        public string ExpectedOutcome { get; init; }

        /// <summary>How the evidence is produced on this machine.</summary>
        public EvidenceLabel Label { get; init; }

        /// <summary>For Fixture / Harness / Simulated rows: exactly what to rerun on the real topology.</summary>
        public string RerunPlan { get; init; }

        /// <summary>The test identity alias used (invented, never a real account).</summary>
        public string Alias { get; set; } = "—";

        /// <summary>The entry point the run used ("intranet route · http://localhost:6408").</summary>
        public string EntryPoint { get; set; } = "—";

        /// <summary>What happened, in words (no identifiers beyond the redacted ones).</summary>
        public string Actual { get; set; } = "";

        /// <summary>The outcome chip after the run ("Deny"), "Failed" when the check failed, or "" before.</summary>
        public string Outcome { get; set; } = "";

        /// <summary>The boundary that decided ("Policy", "Ingress", "OS token", …).</summary>
        public string Boundary { get; set; } = "";

        /// <summary>Repository (or scoped file) calls the case caused; every deny must show 0.</summary>
        public int RepositoryCalls { get; set; }

        /// <summary>The correlation label from <c>ILogger.BeginScope</c> ("rr-07-3f9a1c"): the audit rows of the case carry it.</summary>
        public string Correlation { get; set; } = "";

        /// <summary>Running now.</summary>
        public bool Running { get; set; }

        /// <summary>True once the case ran (passed or failed).</summary>
        public bool Done { get; set; }

        /// <summary>True when the actual outcome matched the expected one.</summary>
        public bool Passed { get; set; }

        /// <summary>The stop condition this run hit, or null.</summary>
        public string StopCondition { get; set; }

        /// <summary>True for a deny (Deny or Safe deny) - the "Denied at a boundary" KPI.</summary>
        public bool IsDeny => this.Outcome is "Deny" or "Safe deny";
    }
}
#endif
